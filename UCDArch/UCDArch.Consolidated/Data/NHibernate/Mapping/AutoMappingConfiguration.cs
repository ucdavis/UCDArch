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
            return new AutoMappingConfiguration { _fluentConfiguration = GetFluentCofiguration(autoPersistenceModel) };
        }

        public static IMappingConfiguration CreateWithOverrides<TClassInDomainObjectAssembly, TClassInMappingAssembly>()
        {
            var autoPersistenceModel =
                new AutoPersistenceModelGenerator().GenerateFromAssembly
                    <TClassInDomainObjectAssembly, TClassInMappingAssembly>();

            return new AutoMappingConfiguration { _fluentConfiguration = GetFluentCofiguration(autoPersistenceModel) };
        }

        private static FluentConfiguration GetFluentCofiguration(AutoPersistenceModel autoPersistenceModel)
        {
            var configuration = SmartServiceLocator<IConfiguration>.GetService();
            var fluentConfiguration = Fluently.Configure()
                .Database(PersistenceConfiguration.GetConfigurer())
                .Mappings(m => m.AutoMappings.Add(autoPersistenceModel));

            return fluentConfiguration;
        }

        public ISessionFactory BuildSessionFactory()
        {
            return _fluentConfiguration.BuildSessionFactory();
        }
    }
}