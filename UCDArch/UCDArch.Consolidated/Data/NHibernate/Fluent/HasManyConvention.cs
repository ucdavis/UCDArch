using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace UCDArch.Data.NHibernate.Fluent
{
    public class HasManyConvention : IHasManyConvention
    {
        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Key.Column(instance.EntityType.Name + "ID");
        }
    }
}