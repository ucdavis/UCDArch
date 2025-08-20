using NHibernate;
using NHibernate.Stat;
using UCDArch.Core.Utils;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UCDArch.Core;

namespace UCDArch.Data.NHibernate
{
    /// <summary>
    /// Handles creation and management of sessions and transactions.  It is a singleton because 
    /// building the initial session factory is very expensive. Inspiration for this class came 
    /// from Chapter 8 of Hibernate in Action by Bauer and King.  Although it is a sealed singleton
    /// you can use TypeMock (http://www.typemock.com) for more flexible testing.
    /// </summary>
    public sealed class NHibernateSessionManager
    {
        #region Thread-safe, lazy Singleton

        private static AsyncLocal<Dictionary<object, object>> _threadSessionMap = new();

        private static Dictionary<object, object> ThreadSessionMap => _threadSessionMap.Value ??= new Dictionary<object, object>();

        /// <summary>
        /// This is a thread-safe, lazy singleton.  See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>
        public static NHibernateSessionManager Instance
        {
            get
            {
                return Nested.NHibernateSessionManager;
            }
        }

        /// <summary>
        /// Determines if we want to use an interceptor within this nhibernate session
        /// </summary>
        private static IInterceptor _registeredInterceptor;

        /// <summary>
        /// Sets the flush mode that all sessions will use.  Default is FlushMode.Never
        /// </summary>
        /// <remarks>
        /// FlushMode.Never means Session.Flush needs to be called to flush changes
        /// </remarks>
        private static FlushMode _flushMode = FlushMode.Manual;

        /// <summary>
        /// Initializes the NHibernate session factory upon instantiation.
        /// </summary>
        private NHibernateSessionManager()
        {
            InitSessionFactory();
        }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        private class Nested
        {
            static Nested() { }
            internal static readonly NHibernateSessionManager NHibernateSessionManager =
                new NHibernateSessionManager();
        }

        #endregion

        private void InitSessionFactory()
        {
            //new Configuration().Configure().BuildSessionFactory();
            sessionFactory = NHibernateSessionConfiguration.MappingConfiguration.BuildSessionFactory();
        }

        /// <summary>
        /// Allows you to set the interceptor which will be used in all subsequent sessions
        /// </summary>
        public void RegisterInterceptor(IInterceptor interceptor)
        {
            Check.Require(interceptor != null, "interceptor may not be null");

            _registeredInterceptor = interceptor;
        }

        /// <summary>
        /// Allows you to set the flush mode which will be used with all sessions
        /// </summary>
        public void SetFlushMode(FlushMode flushMode)
        {
            _flushMode = flushMode;
        }

        /// <summary>
        /// Shortcut which just calls into the GetSession overload with the current registered interceptor.
        /// If no interceptor has been registered, it will be null
        /// </summary>
        public ISession GetSession()
        {
            return GetSession(_registeredInterceptor);
        }

        /// <summary>
        /// Gets a session with or without an interceptor.  This method is not called directly; instead,
        /// it gets invoked from other public methods.
        /// </summary>
        private ISession GetSession(IInterceptor interceptor)
        {
            ISession session = ThreadSession;

            if (session == null)
            {
                session = interceptor != null ? sessionFactory.WithOptions().Interceptor(interceptor).OpenSession() : sessionFactory.OpenSession();

                session.FlushMode = _flushMode;

                ThreadSession = session;
            }

            Check.Ensure(session != null, "session was null");

            return session;
        }

        public void CloseSession()
        {
            ISession session = ThreadSession;
            ThreadSession = null;

            if (session != null && session.IsOpen)
            {
                session.Close();
            }
        }

        public void BeginTransaction()
        {
            ITransaction transaction = ThreadTransaction;

            if (transaction == null)
            {
                transaction = GetSession().BeginTransaction();
                ThreadTransaction = transaction;
            }
        }

        public void CommitTransaction()
        {
            ITransaction transaction = ThreadTransaction;

            try
            {
                if (HasOpenTransaction())
                {
                    transaction.Commit();
                    ThreadTransaction = null;
                }
            }
            catch (HibernateException)
            {
                RollbackTransaction();
                throw;
            }
        }

        public bool HasOpenTransaction()
        {
            ITransaction transaction = ThreadTransaction;

            return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
        }

        public void RollbackTransaction()
        {
            ITransaction transaction = ThreadTransaction;

            try
            {
                ThreadTransaction = null;

                if (HasOpenTransaction())
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                CloseSession();
            }
        }

        /// <summary>
        /// Make sure the object given is connected to the current session
        /// </summary>
        /// <param name="o">Mapped Object</param>
        public void EnsureFreshness(object o)
        {
            if (GetSession().Contains(o) == false) //check is the current session contains the desired object
                GetSession().Refresh(o);
        }

        public IStatistics FactoryStatistics
        {
            get
            {
                Check.Require(sessionFactory != null, "Session factory is null");
                return sessionFactory.Statistics;
            }
        }

        /// <summary>
        /// If within a web context, this uses <see cref="HttpContext" /> instead of the WinForms 
        /// specific <see cref="CallContext" />.  Discussion concerning this found at 
        /// http://forum.springframework.net/showthread.php?t=572.
        /// </summary>
        private ITransaction ThreadTransaction
        {
            get
            {
                if (IsInWebContext())
                {
                    return (ITransaction)SmartServiceLocator<Microsoft.AspNetCore.Http.IHttpContextAccessor>.GetService().HttpContext.Items[TRANSACTION_KEY];
                }
                else
                {
                    return (ITransaction)ThreadSessionMap[TRANSACTION_KEY];
                }
            }
            set
            {
                if (IsInWebContext())
                {
                    SmartServiceLocator<Microsoft.AspNetCore.Http.IHttpContextAccessor>.GetService().HttpContext.Items[TRANSACTION_KEY] = value;
                }
                else
                {
                    ThreadSessionMap[TRANSACTION_KEY] = value;
                }
            }
        }

        /// <summary>
        /// If within a web context, this uses <see cref="HttpContext" /> instead of the WinForms 
        /// specific <see cref="CallContext" />.  Discussion concerning this found at 
        /// http://forum.springframework.net/showthread.php?t=572.
        /// </summary>
        private ISession ThreadSession
        {
            get
            {
                if (IsInWebContext())
                {
                    SmartServiceLocator<Microsoft.AspNetCore.Http.IHttpContextAccessor>.GetService().HttpContext.Items.TryGetValue(SESSION_KEY, out var session);
                    return (ISession)session;
                }
                else
                {
                    return (ISession)ThreadSessionMap[SESSION_KEY];
                }
            }
            set
            {
                if (IsInWebContext())
                {
                    SmartServiceLocator<Microsoft.AspNetCore.Http.IHttpContextAccessor>.GetService().HttpContext.Items[SESSION_KEY] = value;
                }
                else
                {
                    ThreadSessionMap[SESSION_KEY] = value;
                }
            }
        }

        private bool IsInWebContext()
        {
            return SmartServiceLocator<Microsoft.AspNetCore.Http.IHttpContextAccessor>.GetService().HttpContext != null;
        }

        private const string TRANSACTION_KEY = "CONTEXT_TRANSACTION";
        private const string SESSION_KEY = "CONTEXT_SESSION";
        private ISessionFactory sessionFactory;
    }
}