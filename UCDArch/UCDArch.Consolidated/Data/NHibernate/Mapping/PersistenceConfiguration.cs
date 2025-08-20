using System;
using FluentNHibernate.Cfg.Db;
using UCDArch.Core;
using Microsoft.Extensions.Configuration;

namespace UCDArch.Data.NHibernate.Mapping
{
    public static class PersistenceConfiguration
    {
        public static IPersistenceConfigurer GetConfigurer()
        {
            var configuration = SmartServiceLocator<IConfiguration>.GetService();
            if (configuration.GetValue<bool>("MainDB:IsSqlite", false))
            {
                var sqliteConfig = SQLiteConfiguration.Standard
                    .ConnectionString(configuration["ConnectionStrings:MainDB"])
                    .AdoNetBatchSize(configuration.GetValue<int>("MainDB:BatchSize", 25));
                var properties = sqliteConfig.ToProperties();
                // ensure in-memory db is not dropped on every session flush
                properties["connection.release_mode"] = "on_close";
                return sqliteConfig;
            }
            else
            {
                return MsSqlConfiguration.MsSql2008
                    .DefaultSchema(configuration["MainDB:Schema"])
                    .ConnectionString(configuration["ConnectionStrings:MainDB"])
                    .AdoNetBatchSize(configuration.GetValue<int>("MainDB:BatchSize", 25));
            }
        }
    }
}
