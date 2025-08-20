using NHibernate;
using NHibernate.Cfg;

namespace UCDArch.Data.NHibernate.Mapping
{
    public class HbmMappingConfiguration : IMappingConfiguration
    {
        public ISessionFactory BuildSessionFactory()
        {
            return new Configuration().Configure().BuildSessionFactory();
        }
    }
}