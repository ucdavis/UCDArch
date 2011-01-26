using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using UCDArch.Core.Utils;

namespace UCDArch.Data.NHibernate.Fluent
{
    public class TableNameConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table(Inflector.Pluralize(instance.EntityType.Name));
        }
    }
}