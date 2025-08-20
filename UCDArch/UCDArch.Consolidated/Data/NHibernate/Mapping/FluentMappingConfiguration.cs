using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using NHibernate;
using UCDArch.Core;
using UCDArch.Data.NHibernate.Fluent;

namespace UCDArch.Data.NHibernate.Mapping
{
    public class FluentMappingConfiguration : IMappingConfiguration
    {
        private FluentConfiguration _fluentConfiguration;

        public static IMappingConfiguration Create(Assembly mappingAssembly)
        {
            return Create(mappingAssembly, typeof (PrimaryKeyConvention).Assembly);
        }

        public static IMappingConfiguration Create(Assembly mappingAssembly, Assembly conventionAssembly)
        {
            var configuration = SmartServiceLocator<IConfiguration>.GetService();
            var fluentConfiguration = Fluently.Configure()
                .Database(PersistenceConfiguration.GetConfigurer())
                .Mappings(x => x.FluentMappings.AddFromAssembly(mappingAssembly)
                       .Conventions.AddAssembly(conventionAssembly));

            return new FluentMappingConfiguration {_fluentConfiguration = fluentConfiguration};
        }

        public ISessionFactory BuildSessionFactory()
        {
            return _fluentConfiguration.BuildSessionFactory();
        }
    }
}