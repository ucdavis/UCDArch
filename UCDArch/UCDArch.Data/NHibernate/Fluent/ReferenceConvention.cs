using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace UCDArch.Data.NHibernate.Fluent
{
    public class ReferenceConvention : IReferenceConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Column(instance.Class.Name + "ID");
        }
    }
}