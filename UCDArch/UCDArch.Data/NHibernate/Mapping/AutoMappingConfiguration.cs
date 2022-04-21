using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using NHibernate;
using UCDArch.Core;
using UCDArch.Data.NHibernate.Fluent;

namespace UCDArch.Data.NHibernate.Mapping
{
    public class AutoMappingConfiguration : IMappingConfiguration
    {
        private FluentConfiguration _fluentConfiguration;

        public static IMappingConfiguration CreateWithoutOverrides(Assembly mappingAssembly)
        {
            var autoPersistenceModel =
                new AutoPersistenceModelGenerator().GenerateFromAssembly(mappingAssembly);

            var configuration = Fluently.Configure()
                .Mappings(m => m.AutoMappings.Add(autoPersistenceModel));

            return new AutoMappingConfiguration { _fluentConfiguration = configuration };
        }

        public static IMappingConfiguration CreateWithOverrides(AutoPersistenceModel autoPersistenceModel)
        {
            var configuration = SmartServiceLocator<IConfiguration>.GetService();
            var fluentConfiguration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008
                    .DefaultSchema(configuration["MainDB:Schema"])
                    .ConnectionString(configuration["ConnectionStrings:MainDB"])
                    .AdoNetBatchSize(configuration.GetValue<int>("MainDB:BatchSize", 25)))
                .Mappings(m => m.AutoMappings.Add(autoPersistenceModel));

            return new AutoMappingConfiguration { _fluentConfiguration = fluentConfiguration };
        }

        public static IMappingConfiguration CreateWithOverrides<TClassInDomainObjectAssembly, TClassInMappingAssembly>()
        {
            var autoPersistenceModel =
                new AutoPersistenceModelGenerator().GenerateFromAssembly
                    <TClassInDomainObjectAssembly, TClassInMappingAssembly>();

            var configuration = SmartServiceLocator<IConfiguration>.GetService();
            var fluentConfiguration = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008
                    .DefaultSchema(configuration["MainDB:Schema"])
                    .ConnectionString(configuration["ConnectionStrings:MainDB"])
                    .AdoNetBatchSize(configuration.GetValue<int>("MainDB:BatchSize", 25)))
                .Mappings(m => m.AutoMappings.Add(autoPersistenceModel));

            return new AutoMappingConfiguration { _fluentConfiguration = fluentConfiguration };
        }

        public ISessionFactory BuildSessionFactory()
        {
            return _fluentConfiguration.BuildSessionFactory();
        }
    }
}