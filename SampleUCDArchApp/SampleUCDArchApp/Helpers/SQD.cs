using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Iesi.Collections.Generic;
using NHibernate;
using NHibernate.AdoNet;
using NHibernate.AdoNet.Util;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace SampleUCDArchApp.Helpers
{
    /// <summary>
    /// A NHibernate Driver for using the SqlClient DataProvider
    /// </summary>
    public class SqlClientDriver : DriverBase, IEmbeddedBatcherFactoryProvider
    {
        /// <summary>
        /// Creates an uninitialized <see cref="SqlConnection" /> object for 
        /// the SqlClientDriver.
        /// </summary>
        /// <value>An unitialized <see cref="System.Data.SqlClient"/> object.</value>
        public override IDbConnection CreateConnection()
        {
            return MvcMiniProfiler.Data.ProfiledDbConnection.Get(new SqlConnection());

            //return new SqlConnection();
        }

        /// <summary>
        /// Creates an uninitialized <see cref="IDbCommand" /> object for 
        /// the SqlClientDriver.
        /// </summary>
        /// <value>An unitialized <see cref="System.Data.SqlClient.SqlCommand"/> object.</value>
        public override IDbCommand CreateCommand()
        {
            return new System.Data.SqlClient.SqlCommand();
        }

        /// <summary>
        /// MsSql requires the use of a Named Prefix in the SQL statement.  
        /// </summary>
        /// <remarks>
        /// <see langword="true" /> because MsSql uses "<c>@</c>".
        /// </remarks>
        public override bool UseNamedPrefixInSql
        {
            get { return true; }
        }

        /// <summary>
        /// MsSql requires the use of a Named Prefix in the Parameter.  
        /// </summary>
        /// <remarks>
        /// <see langword="true" /> because MsSql uses "<c>@</c>".
        /// </remarks>
        public override bool UseNamedPrefixInParameter
        {
            get { return true; }
        }

        /// <summary>
        /// The Named Prefix for parameters.  
        /// </summary>
        /// <value>
        /// Sql Server uses <c>"@"</c>.
        /// </value>
        public override string NamedPrefix
        {
            get { return "@"; }
        }

        /// <summary>
        /// The SqlClient driver does NOT support more than 1 open IDataReader
        /// with only 1 IDbConnection.
        /// </summary>
        /// <value><see langword="false" /> - it is not supported.</value>
        /// <remarks>
        /// MS SQL Server 2000 (and 7) throws an exception when multiple IDataReaders are 
        /// attempted to be opened.  When SQL Server 2005 comes out a new driver will be 
        /// created for it because SQL Server 2005 is supposed to support it.
        /// </remarks>
        public override bool SupportsMultipleOpenReaders
        {
            get { return false; }
        }

        // Used from SqlServerCeDriver as well
        public static void SetParameterSizes(IDataParameterCollection parameters, SqlType[] parameterTypes)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                SetVariableLengthParameterSize((IDbDataParameter)parameters[i], parameterTypes[i]);
            }
        }

        private const int MaxAnsiStringSize = 8000;
        private const int MaxBinarySize = MaxAnsiStringSize;
        private const int MaxStringSize = MaxAnsiStringSize / 2;
        private const int MaxBinaryBlobSize = int.MaxValue;
        private const int MaxStringClobSize = MaxBinaryBlobSize / 2;
        private const byte MaxPrecision = 28;
        private const byte MaxScale = 5;
        private const byte MaxDateTime2 = 8;
        private const byte MaxDateTimeOffset = 10;

        private static void SetDefaultParameterSize(IDbDataParameter dbParam, SqlType sqlType)
        {
            switch (dbParam.DbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                    dbParam.Size = MaxAnsiStringSize;
                    break;

                case DbType.Binary:
                    if (sqlType is BinaryBlobSqlType)
                    {
                        dbParam.Size = MaxBinaryBlobSize;
                    }
                    else
                    {
                        dbParam.Size = MaxBinarySize;
                    }
                    break;
                case DbType.Decimal:
                    dbParam.Precision = MaxPrecision;
                    dbParam.Scale = MaxScale;
                    break;
                case DbType.String:
                case DbType.StringFixedLength:
                    dbParam.Size = IsText(dbParam, sqlType) ? MaxStringClobSize : MaxStringSize;
                    break;
                case DbType.DateTime2:
                    dbParam.Size = MaxDateTime2;
                    break;
                case DbType.DateTimeOffset:
                    dbParam.Size = MaxDateTimeOffset;
                    break;
            }
        }

        private static bool IsText(IDbDataParameter dbParam, SqlType sqlType)
        {
            return (sqlType is StringClobSqlType) || (sqlType.LengthDefined &&
                (DbType.String == dbParam.DbType || DbType.StringFixedLength == dbParam.DbType));
        }

        private static void SetVariableLengthParameterSize(IDbDataParameter dbParam, SqlType sqlType)
        {
            SetDefaultParameterSize(dbParam, sqlType);

            // Override the defaults using data from SqlType.
            if (sqlType.LengthDefined && !IsText(dbParam, sqlType))
            {
                dbParam.Size = sqlType.Length;
            }

            if (sqlType.PrecisionDefined)
            {
                dbParam.Precision = sqlType.Precision;
                dbParam.Scale = sqlType.Scale;
            }
        }

        public override IDbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
        {
            IDbCommand command = base.GenerateCommand(type, sqlString, parameterTypes);
            //if (IsPrepareSqlEnabled)
            {
                SetParameterSizes(command.Parameters, parameterTypes);
            }
            return command;
        }

        public override bool SupportsMultipleQueries
        {
            get { return true; }
        }

        #region IEmbeddedBatcherFactoryProvider Members

        System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass
        {
            //get { return typeof(SqlClientBatchingBatcherFactory); }
            get { return typeof (MyBatcherFactory); }
        }

        #endregion
    }

    public class MyBatcherFactory : SqlClientBatchingBatcherFactory
    {
        public override NHibernate.Engine.IBatcher CreateBatcher(ConnectionManager connectionManager, NHibernate.IInterceptor interceptor)
        {
            return new MyBatcher(connectionManager, interceptor);
        }
    }

    public class MyBatcher : MySqlClientBatchingBatcher
    {
        public MyBatcher(ConnectionManager connectionManager, IInterceptor interceptor) : base(connectionManager, interceptor)
        {

        }
    }

    /// <summary>
    /// Summary description for SqlClientBatchingBatcher.
    /// </summary>
    public class MySqlClientBatchingBatcher : MyAbstractBatcher
    {
        private int batchSize;
        private int totalExpectedRowsAffected;
        private SqlClientSqlCommandSet currentBatch;
        private StringBuilder currentBatchCommandsLog;
        private readonly int defaultTimeout;

        public MySqlClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
            : base(connectionManager, interceptor)
        {
            batchSize = Factory.Settings.AdoBatchSize;
            defaultTimeout = PropertiesHelper.GetInt32(NHibernate.Cfg.Environment.CommandTimeout, NHibernate.Cfg.Environment.Properties, -1);

            currentBatch = CreateConfiguredBatch();
            //we always create this, because we need to deal with a scenario in which
            //the user change the logging configuration at runtime. Trying to put this
            //behind an if(log.IsDebugEnabled) will cause a null reference exception 
            //at that point.
            currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
        }

        public override int BatchSize
        {
            get { return batchSize; }
            set { batchSize = value; }
        }

        protected override int CountOfStatementsInCurrentBatch
        {
            get { return currentBatch.CountOfCommands; }
        }

        public override void AddToBatch(IExpectation expectation)
        {
            totalExpectedRowsAffected += expectation.ExpectedRowCount;
            IDbCommand batchUpdate = CurrentCommand;

            string lineWithParameters = null;
            var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
            if (sqlStatementLogger.IsDebugEnabled || log.IsDebugEnabled)
            {
                lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
                var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
                lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
                currentBatchCommandsLog.Append("command ")
                    .Append(currentBatch.CountOfCommands)
                    .Append(":")
                    .AppendLine(lineWithParameters);
            }
            if (log.IsDebugEnabled)
            {
                log.Debug("Adding to batch:" + lineWithParameters);
            }
            currentBatch.Append((System.Data.SqlClient.SqlCommand)batchUpdate);

            if (currentBatch.CountOfCommands >= batchSize)
            {
                ExecuteBatchWithTiming(batchUpdate);
            }
        }

        protected override void DoExecuteBatch(IDbCommand ps)
        {
            log.DebugFormat("Executing batch");
            CheckReaders();
            Prepare(currentBatch.BatchCommand);
            if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
            {
                Factory.Settings.SqlStatementLogger.LogBatchCommand(currentBatchCommandsLog.ToString());
                currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
            }

            int rowsAffected;
            try
            {
                rowsAffected = currentBatch.ExecuteNonQuery();
            }
            catch (DbException e)
            {
                throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not execute batch command.");
            }

            Expectations.VerifyOutcomeBatched(totalExpectedRowsAffected, rowsAffected);

            currentBatch.Dispose();
            totalExpectedRowsAffected = 0;
            currentBatch = CreateConfiguredBatch();
        }

        private SqlClientSqlCommandSet CreateConfiguredBatch()
        {
            var result = new SqlClientSqlCommandSet();
            if (defaultTimeout > 0)
            {
                try
                {
                    result.CommandTimeout = defaultTimeout;
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                    {
                        log.Warn(e.ToString());
                    }
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Manages prepared statements and batching. Class exists to enforce separation of concerns
    /// </summary>
    public abstract class MyAbstractBatcher : IBatcher
    {
        protected static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(AbstractBatcher));

        private static int openCommandCount;
        private static int openReaderCount;

        private readonly ConnectionManager connectionManager;
        private readonly ISessionFactoryImplementor factory;

        // batchCommand used to be called batchUpdate - that name to me implied that updates
        // were being sent - however this could be just INSERT/DELETE/SELECT SQL statement not
        // just update.  However I haven't seen this being used with read statements...
        private IDbCommand batchCommand;
        private SqlString batchCommandSql;
        private SqlType[] batchCommandParameterTypes;

        private readonly HashedSet<IDbCommand> commandsToClose = new HashedSet<IDbCommand>();
        private readonly HashedSet<IDataReader> readersToClose = new HashedSet<IDataReader>();
        private readonly IDictionary<IDataReader, Stopwatch> readersDuration = new Dictionary<IDataReader, Stopwatch>();
        private IDbCommand lastQuery;

        private bool releasing;

        private readonly IInterceptor interceptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBatcher"/> class.
        /// </summary>
        /// <param name="connectionManager">The <see cref="ConnectionManager"/> owning this batcher.</param>
        /// <param name="interceptor"></param>
        protected MyAbstractBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
        {
            this.connectionManager = connectionManager;
            this.interceptor = interceptor;
            factory = connectionManager.Factory;
        }

        private IDriver Driver
        {
            get { return factory.ConnectionProvider.Driver; }
        }

        /// <summary>
        /// Gets the current <see cref="IDbCommand"/> that is contained for this Batch
        /// </summary>
        /// <value>The current <see cref="IDbCommand"/>.</value>
        protected IDbCommand CurrentCommand
        {
            get { return batchCommand; }
        }

        public IDbCommand Generate(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
        {
            SqlString sql = GetSQL(sqlString);

            IDbCommand cmd = factory.ConnectionProvider.Driver.GenerateCommand(type, sql, parameterTypes);
            LogOpenPreparedCommand();
            if (log.IsDebugEnabled)
            {
                log.Debug("Building an IDbCommand object for the SqlString: " + sql);
            }
            commandsToClose.Add(cmd);
            return cmd;
        }

        /// <summary>
        /// Prepares the <see cref="IDbCommand"/> for execution in the database.
        /// </summary>
        /// <remarks>
        /// This takes care of hooking the <see cref="IDbCommand"/> up to an <see cref="IDbConnection"/>
        /// and <see cref="IDbTransaction"/> if one exists.  It will call <c>Prepare</c> if the Driver
        /// supports preparing commands.
        /// </remarks>
        protected void Prepare(IDbCommand cmd)
        {
            try
            {
                IDbConnection sessionConnection = connectionManager.GetConnection();

                if (cmd.Connection != null)
                {
                    // make sure the commands connection is the same as the Sessions connection
                    // these can be different when the session is disconnected and then reconnected
                    if (cmd.Connection != sessionConnection)
                    {
                        cmd.Connection = sessionConnection;
                    }
                }
                else
                {
                    cmd.Connection = sessionConnection;
                }

                connectionManager.Transaction.Enlist(cmd);
                Driver.PrepareCommand(cmd);
            }
            catch (InvalidOperationException ioe)
            {
                throw new ADOException("While preparing " + cmd.CommandText + " an error occurred", ioe);
            }
        }

        public IDbCommand PrepareBatchCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
        {
            /* NH:
             * The code inside this block was added for a strange behaviour
             * discovered using Firebird (some times for us is external issue).
             * The problem is that batchCommandSql as a value, batchCommand is not null
             * BUT batchCommand.CommandText is null (I don't know who clear it)
             */
            bool forceCommandRecreate = batchCommand == null || string.IsNullOrEmpty(batchCommand.CommandText);
            /****************************************/
            if (sql.Equals(batchCommandSql) &&
                ArrayHelper.ArrayEquals(parameterTypes, batchCommandParameterTypes) && !forceCommandRecreate)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("reusing command " + batchCommand.CommandText);
                }
            }
            else
            {
                batchCommand = PrepareCommand(type, sql, parameterTypes); // calls ExecuteBatch()
                batchCommandSql = sql;
                batchCommandParameterTypes = parameterTypes;
            }

            return batchCommand;
        }

        public IDbCommand PrepareCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
        {
            OnPreparedCommand();

            // do not actually prepare the Command here - instead just generate it because
            // if the command is associated with an ADO.NET Transaction/Connection while
            // another open one Command is doing something then an exception will be 
            // thrown.
            return Generate(type, sql, parameterTypes);
        }

        protected virtual void OnPreparedCommand()
        {
            // a new IDbCommand is being prepared and a new (potential) batch
            // started - so execute the current batch of commands.
            ExecuteBatch();
        }

        public IDbCommand PrepareQueryCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
        {
            // do not actually prepare the Command here - instead just generate it because
            // if the command is associated with an ADO.NET Transaction/Connection while
            // another open one Command is doing something then an exception will be 
            // thrown.
            IDbCommand command = Generate(type, sql, parameterTypes);
            lastQuery = command;
            return command;
        }

        public void AbortBatch(Exception e)
        {
            IDbCommand cmd = batchCommand;
            InvalidateBatchCommand();
            // close the statement closeStatement(cmd)
            if (cmd != null)
            {
                CloseCommand(cmd, null);
            }
        }

        private void InvalidateBatchCommand()
        {
            batchCommand = null;
            batchCommandSql = null;
            batchCommandParameterTypes = null;
        }

        public int ExecuteNonQuery(IDbCommand cmd)
        {
            CheckReaders();
            LogCommand(cmd);
            Prepare(cmd);
            Stopwatch duration = null;
            if (log.IsDebugEnabled)
                duration = Stopwatch.StartNew();
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                e.Data["actual-sql-query"] = cmd.CommandText;
                log.Error("Could not execute command: " + cmd.CommandText, e);
                throw;
            }
            finally
            {
                if (log.IsDebugEnabled && duration != null)
                    log.DebugFormat("ExecuteNonQuery took {0} ms", duration.ElapsedMilliseconds);
            }
        }

        public void ExpandQueryParameters(IDbCommand cmd, SqlString sqlString)
        {
            Driver.ExpandQueryParameters(cmd, sqlString);
        }

        public IDataReader ExecuteReader(IDbCommand cmd)
        {
            CheckReaders();
            LogCommand(cmd);
            Prepare(cmd);
            Stopwatch duration = null;
            if (log.IsDebugEnabled)
                duration = Stopwatch.StartNew();
            IDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                e.Data["actual-sql-query"] = cmd.CommandText;
                log.Error("Could not execute query: " + cmd.CommandText, e);
                throw;
            }
            finally
            {
                if (log.IsDebugEnabled && duration != null && reader != null)
                {
                    log.DebugFormat("ExecuteReader took {0} ms", duration.ElapsedMilliseconds);
                    readersDuration[reader] = duration;
                }
            }

            if (!factory.ConnectionProvider.Driver.SupportsMultipleOpenReaders)
            {
                reader = new NHybridDataReader(reader);
            }

            readersToClose.Add(reader);
            LogOpenReader();
            return reader;
        }

        /// <summary>
        /// Ensures that the Driver's rules for Multiple Open DataReaders are being followed.
        /// </summary>
        protected void CheckReaders()
        {
            // early exit because we don't need to move an open IDataReader into memory
            // since the Driver supports mult open readers.
            if (factory.ConnectionProvider.Driver.SupportsMultipleOpenReaders)
            {
                return;
            }

            foreach (NHybridDataReader reader in readersToClose)
            {
                reader.ReadIntoMemory();
            }
        }

        public void CloseCommands()
        {
            releasing = true;
            try
            {
                foreach (IDataReader reader in new HashedSet<IDataReader>(readersToClose))
                {
                    try
                    {
                        CloseReader(reader);
                    }
                    catch (Exception e)
                    {
                        log.Warn("Could not close IDataReader", e);
                    }
                }

                foreach (IDbCommand cmd in commandsToClose)
                {
                    try
                    {
                        CloseCommand(cmd);
                    }
                    catch (Exception e)
                    {
                        // no big deal
                        log.Warn("Could not close ADO.NET Command", e);
                    }
                }
                commandsToClose.Clear();
            }
            finally
            {
                releasing = false;
            }
        }

        private void CloseCommand(IDbCommand cmd)
        {
            try
            {
                // no equiv to the java code in here
                cmd.Dispose();
                LogClosePreparedCommand();
            }
            catch (Exception e)
            {
                log.Warn("exception clearing maxRows/queryTimeout", e);
                return; // NOTE: early exit!
            }
            finally
            {
                if (!releasing)
                {
                    connectionManager.AfterStatement();
                }
            }

            if (lastQuery == cmd)
            {
                lastQuery = null;
            }
        }

        public void CloseCommand(IDbCommand st, IDataReader reader)
        {
            commandsToClose.Remove(st);
            try
            {
                CloseReader(reader);
            }
            finally
            {
                CloseCommand(st);
            }
        }

        public void CloseReader(IDataReader reader)
        {
            /* This method was added because PrepareCommand don't really prepare the command
             * with its connection. 
             * In some case we need to manage a reader outsite the command scope. 
             * To do it we need to use the Batcher.ExecuteReader and then we need something
             * to close the opened reader.
             */
            // TODO NH: Study a way to use directly IDbCommand.ExecuteReader() outsite the batcher
            // An example of it's use is the management of generated ID.
            if (reader == null)
                return;

            MyResultSetWrapper rsw = reader as MyResultSetWrapper;
            var actualReader = rsw == null ? reader : rsw.Target;
            readersToClose.Remove(actualReader);

            try
            {
                reader.Dispose();
            }
            catch (Exception e)
            {
                // NH2205 - prevent exceptions when closing the reader from hiding any original exception
                log.Warn("exception closing reader", e);
            }

            LogCloseReader();

            if (!log.IsDebugEnabled)
                return;

            var nhReader = actualReader as NHybridDataReader;
            actualReader = nhReader == null ? actualReader : nhReader.Target;

            Stopwatch duration;
            if (readersDuration.TryGetValue(actualReader, out duration) == false)
                return;
            readersDuration.Remove(actualReader);
            log.DebugFormat("DataReader was closed after {0} ms", duration.ElapsedMilliseconds);
        }

        /// <summary></summary>
        public void ExecuteBatch()
        {
            // if there is currently a command that a batch is
            // being built for then execute it
            if (batchCommand != null)
            {
                IDbCommand ps = batchCommand;
                InvalidateBatchCommand();
                try
                {
                    ExecuteBatchWithTiming(ps);
                }
                finally
                {
                    CloseCommand(ps, null);
                }
            }
        }

        protected void ExecuteBatchWithTiming(IDbCommand ps)
        {
            Stopwatch duration = null;
            if (log.IsDebugEnabled)
                duration = Stopwatch.StartNew();
            var countBeforeExecutingBatch = CountOfStatementsInCurrentBatch;
            DoExecuteBatch(ps);
            if (log.IsDebugEnabled && duration != null)
                log.DebugFormat("ExecuteBatch for {0} statements took {1} ms",
                    countBeforeExecutingBatch,
                    duration.ElapsedMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ps"></param>
        protected abstract void DoExecuteBatch(IDbCommand ps);

        protected abstract int CountOfStatementsInCurrentBatch { get; }

        /// <summary>
        /// Gets or sets the size of the batch, this can change dynamically by
        /// calling the session's SetBatchSize.
        /// </summary>
        /// <value>The size of the batch.</value>
        public abstract int BatchSize
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the expected row count into the batch.
        /// </summary>
        /// <param name="expectation">The number of rows expected to be affected by the query.</param>
        /// <remarks>
        /// If Batching is not supported, then this is when the Command should be executed.  If Batching
        /// is supported then it should hold of on executing the batch until explicitly told to.
        /// </remarks>
        public abstract void AddToBatch(IExpectation expectation);

        /// <summary>
        /// Gets the <see cref="ISessionFactoryImplementor"/> the Batcher was
        /// created in.
        /// </summary>
        /// <value>
        /// The <see cref="ISessionFactoryImplementor"/> the Batcher was
        /// created in.
        /// </value>
        protected ISessionFactoryImplementor Factory
        {
            get { return factory; }
        }

        /// <summary>
        /// Gets the <see cref="ConnectionManager"/> for this batcher.
        /// </summary>
        protected ConnectionManager ConnectionManager
        {
            get { return connectionManager; }
        }

        protected void LogCommand(IDbCommand command)
        {
            factory.Settings.SqlStatementLogger.LogCommand(command, FormatStyle.Basic);
        }

        private void LogOpenPreparedCommand()
        {
            if (log.IsDebugEnabled)
            {
                int currentOpenCommandCount = Interlocked.Increment(ref openCommandCount);
                log.Debug("Opened new IDbCommand, open IDbCommands: " + currentOpenCommandCount);
            }

            if (factory.Statistics.IsStatisticsEnabled)
            {
                factory.StatisticsImplementor.PrepareStatement();
            }
        }

        private void LogClosePreparedCommand()
        {
            if (log.IsDebugEnabled)
            {
                int currentOpenCommandCount = Interlocked.Decrement(ref openCommandCount);
                log.Debug("Closed IDbCommand, open IDbCommands: " + currentOpenCommandCount);
            }

            if (factory.Statistics.IsStatisticsEnabled)
            {
                factory.StatisticsImplementor.CloseStatement();
            }
        }

        private static void LogOpenReader()
        {
            if (log.IsDebugEnabled)
            {
                int currentOpenReaderCount = Interlocked.Increment(ref openReaderCount);
                log.Debug("Opened IDataReader, open IDataReaders: " + currentOpenReaderCount);
            }
        }

        private static void LogCloseReader()
        {
            if (log.IsDebugEnabled)
            {
                int currentOpenReaderCount = Interlocked.Decrement(ref openReaderCount);
                log.Debug("Closed IDataReader, open IDataReaders :" + currentOpenReaderCount);
            }
        }

        public void CancelLastQuery()
        {
            try
            {
                if (lastQuery != null)
                {
                    lastQuery.Cancel();
                }
            }
            catch (HibernateException)
            {
                // Do not call Convert on HibernateExceptions
                throw;
            }
            catch (Exception sqle)
            {
                throw Convert(sqle, "Could not cancel query");
            }
        }

        public bool HasOpenResources
        {
            get { return commandsToClose.Count > 0 || readersToClose.Count > 0; }
        }

        protected Exception Convert(Exception sqlException, string message)
        {
            return ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqlException, message);
        }

        #region IDisposable Members

        /// <summary>
        /// A flag to indicate if <c>Dispose()</c> has been called.
        /// </summary>
        private bool _isAlreadyDisposed;

        /// <summary>
        /// Finalizer that ensures the object is correctly disposed of.
        /// </summary>
        ~MyAbstractBatcher()
        {
            // Don't log in the finalizer, it causes problems
            // if the output stream is finalized before the batcher.
            //log.Debug( "running BatcherImpl.Dispose(false)" );
            Dispose(false);
        }

        /// <summary>
        /// Takes care of freeing the managed and unmanaged resources that 
        /// this class is responsible for.
        /// </summary>
        public void Dispose()
        {
            log.Debug("running BatcherImpl.Dispose(true)");
            Dispose(true);
        }

        /// <summary>
        /// Takes care of freeing the managed and unmanaged resources that 
        /// this class is responsible for.
        /// </summary>
        /// <param name="isDisposing">Indicates if this BatcherImpl is being Disposed of or Finalized.</param>
        /// <remarks>
        /// If this BatcherImpl is being Finalized (<c>isDisposing==false</c>) then make sure not
        /// to call any methods that could potentially bring this BatcherImpl back to life.
        /// </remarks>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isAlreadyDisposed)
            {
                // don't dispose of multiple times.
                return;
            }

            // free managed resources that are being managed by the AdoTransaction if we
            // know this call came through Dispose()
            if (isDisposing)
            {
                CloseCommands();
            }

            // free unmanaged resources here

            _isAlreadyDisposed = true;
            // nothing for Finalizer to do - so tell the GC to ignore it
            GC.SuppressFinalize(this);
        }

        #endregion

        protected SqlString GetSQL(SqlString sql)
        {
            sql = interceptor.OnPrepareStatement(sql);
            if (sql == null || sql.Length == 0)
            {
                throw new AssertionFailure("Interceptor.OnPrepareStatement(SqlString) returned null or empty SqlString.");
            }
            return sql;
        }
    }

    /// <summary> 
    /// A ResultSet delegate, responsible for locally caching the columnName-to-columnIndex
    /// resolution that has been found to be inefficient in a few vendor's drivers (i.e., Oracle
    /// and Postgres). 
    /// </summary>
    /// <seealso cref="IDataRecord.GetOrdinal"/>
    public class MyResultSetWrapper : ResultSetWrapper
    {
        private readonly IDataReader rs;
        private readonly ColumnNameCache columnNameCache;

        public MyResultSetWrapper(IDataReader resultSet, ColumnNameCache columnNameCache) : base(resultSet, columnNameCache)
        {
        }

        public IDataReader Target
        {
            get { return rs; }
        }

        #region IDataReader Members

        public void Close()
        {
            rs.Close();
        }

        public DataTable GetSchemaTable()
        {
            return rs.GetSchemaTable();
        }

        public bool NextResult()
        {
            return rs.NextResult();
        }

        public bool Read()
        {
            return rs.Read();
        }

        public int Depth
        {
            get { return rs.Depth; }
        }

        public bool IsClosed
        {
            get { return rs.IsClosed; }
        }

        public int RecordsAffected
        {
            get { return rs.RecordsAffected; }
        }

        #endregion

        #region IDisposable Members
        private bool disposed;

        ~MyResultSetWrapper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (rs != null)
                {
                    if (!rs.IsClosed) rs.Close();
                    rs.Dispose();
                }
            }

            disposed = true;
        }
        #endregion

        #region IDataRecord Members

        public string GetName(int i)
        {
            return rs.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            return rs.GetDataTypeName(i);
        }

        public System.Type GetFieldType(int i)
        {
            return rs.GetFieldType(i);
        }

        public object GetValue(int i)
        {
            return rs.GetDecimal(i);
        }

        public int GetValues(object[] values)
        {
            return rs.GetValues(values);
        }

        public int GetOrdinal(string name)
        {
            return columnNameCache.GetIndexForColumnName(name, this);
        }

        public bool GetBoolean(int i)
        {
            return rs.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return rs.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return rs.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return rs.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return rs.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public Guid GetGuid(int i)
        {
            return rs.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return rs.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return rs.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return rs.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            return rs.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            return rs.GetDouble(i);
        }

        public string GetString(int i)
        {
            return rs.GetString(i);
        }

        public decimal GetDecimal(int i)
        {
            return rs.GetDecimal(i);
        }

        public DateTime GetDateTime(int i)
        {
            return rs.GetDateTime(i);
        }

        public IDataReader GetData(int i)
        {
            return rs.GetData(i);
        }

        public bool IsDBNull(int i)
        {
            return rs.IsDBNull(i);
        }

        public int FieldCount
        {
            get { return rs.FieldCount; }
        }

        public object this[int i]
        {
            get { return rs[i]; }
        }

        public object this[string name]
        {
            get { return rs[columnNameCache.GetIndexForColumnName(name, this)]; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            return rs.Equals(obj);
        }

        public override int GetHashCode()
        {
            return rs.GetHashCode();
        }
    }
}
