using System.Data.Common;
using System.Data.SqlClient;
using MvcMiniProfiler;
using MvcMiniProfiler.Data;
using NHibernate.Connection;

namespace SampleUCDArchApp.Helpers
{
    public class ProfilerConnectionProvider : DriverConnectionProvider
    {
        public override System.Data.IDbConnection GetConnection()
        {
            var connection = (SqlConnection)base.GetConnection();

            //return connection;
            return ProfiledDbConnection.Get(connection, MiniProfiler.Current);
        }
    }

    //public class ProfilerSqlDriver : NHibernate.Driver.SqlClientDriver
    //{
    //    public override System.Data.IDbConnection CreateConnection()
    //    {
    //        var connection = (SqlConnection)base.CreateConnection();

    //        //return connection;
    //        return ProfiledDbConnection.Get(connection, MiniProfiler.Current);
    //    }
       
    //}

    public class AnotherDriver : NHibernate.Driver.OracleClientDriver{}
}