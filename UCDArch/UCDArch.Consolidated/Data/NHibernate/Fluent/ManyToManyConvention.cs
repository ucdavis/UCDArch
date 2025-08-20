using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using UCDArch.Core.Utils;

namespace UCDArch.Data.NHibernate.Fluent
{
    public class ManyToManyConvention : IHasManyToManyConvention
    {
        public void Apply(IManyToManyCollectionInstance instance)
        {
            var entityName = Inflector.Pluralize(instance.EntityType.Name);
            var childName = Inflector.Pluralize(instance.ChildType.Name);

            var tableName = string.Format("{0}X{1}", entityName, childName);

            instance.Table(tableName);
            instance.Key.Column(entityName + "ID");
            instance.Relationship.Column(childName + "ID");
        }
    }
}