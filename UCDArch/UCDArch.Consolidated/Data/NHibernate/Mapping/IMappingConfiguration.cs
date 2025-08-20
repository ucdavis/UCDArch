using NHibernate;

namespace UCDArch.Data.NHibernate.Mapping
{
    public interface IMappingConfiguration
    {
        ISessionFactory BuildSessionFactory();
    }
}