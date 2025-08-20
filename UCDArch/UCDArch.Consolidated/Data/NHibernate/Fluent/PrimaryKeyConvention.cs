using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace UCDArch.Data.NHibernate.Fluent
{
    public class PrimaryKeyConvention : IIdConvention
    {
        public void Apply(IIdentityInstance instance)
        {
            instance.Column("ID");
        }
    }
}