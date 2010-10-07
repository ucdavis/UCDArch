using System.Reflection;
using FluentNHibernate.Cfg;
using NHibernate;

namespace UCDArch.Data.NHibernate.Mapping
{
    public class FluentMappingConfiguration : IMappingConfiguration
    {
        private FluentConfiguration _fluentConfiguration;

        public static IMappingConfiguration Create(Assembly mappingAssembly)
        {
            var configuration = Fluently
                .Configure()
                .Mappings(x => x.FluentMappings.AddFromAssembly(mappingAssembly));

            return new FluentMappingConfiguration { _fluentConfiguration = configuration };
        }

        public ISessionFactory BuildSessionFactory()
        {
            return _fluentConfiguration.BuildSessionFactory();
        }
    }
}