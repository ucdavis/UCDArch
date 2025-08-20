using System.Reflection;
using UCDArch.Data.NHibernate.Mapping;

namespace UCDArch.Data.NHibernate
{
    public static class NHibernateSessionConfiguration
    {
        private static IMappingConfiguration _mappingConfiguration;

        internal static IMappingConfiguration MappingConfiguration
        {
            get
            {
                if (_mappingConfiguration == null) return new HbmMappingConfiguration();

                return _mappingConfiguration;
            }
            private set
            {
                _mappingConfiguration = value;
            }
        }

        public static class Mappings
        {
            /// <summary>
            /// Use HBM mappings with the default NHibernate behavior
            /// </summary>
            public static void UseHbmMappings()
            {
                MappingConfiguration = new HbmMappingConfiguration();
            }
            
            /// <summary>
            /// Use fluent mappings
            /// </summary>
            /// <param name="mappingAssembly">Assembly which contains mapping class files</param>
            public static void UseFluentMappings(Assembly mappingAssembly)
            {
                MappingConfiguration = FluentMappingConfiguration.Create(mappingAssembly);
            }

            public static void UseFluentMappings(Assembly mappingAssembly, Assembly conventionAssembly)
            {
                MappingConfiguration = FluentMappingConfiguration.Create(mappingAssembly, conventionAssembly);
            }

            public static void UseAutoMappings<TClassInDomainObjectAssembly, TClassInMappingAssembly>()
            {
                MappingConfiguration =
                    AutoMappingConfiguration.CreateWithOverrides<TClassInDomainObjectAssembly, TClassInMappingAssembly>();
            }

            public static void UseCustomMapping(IMappingConfiguration mappingConfiguration)
            {
                MappingConfiguration = mappingConfiguration;
            }
        }

    }
}