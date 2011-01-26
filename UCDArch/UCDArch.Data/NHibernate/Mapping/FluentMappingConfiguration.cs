using System.Reflection;
using FluentNHibernate.Cfg;
using NHibernate;
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
            var configuration = Fluently
                .Configure()
                .Mappings(x => x.FluentMappings.AddFromAssembly(mappingAssembly)
                       .Conventions.AddAssembly(conventionAssembly));

            return new FluentMappingConfiguration {_fluentConfiguration = configuration};
        }

        public ISessionFactory BuildSessionFactory()
        {
            return _fluentConfiguration.BuildSessionFactory();
        }
    }
}