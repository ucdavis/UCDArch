using System;
using System.Linq;
using System.Reflection;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;
using UCDArch.Core.DomainModel;

namespace UCDArch.Data.NHibernate.Fluent
{
    public class AutoPersistenceModelGenerator
    {
        public virtual AutoPersistenceModel GenerateFromAssembly(Assembly domainObjectAssembly)
        {
            var mappings = GetAutoPersistenceModelDefaults();

            mappings.AddEntityAssembly(domainObjectAssembly).Where(GetAutoMappingFilter);

            return mappings;
        }

        public virtual AutoPersistenceModel GenerateFromAssembly<TClassInDomainObjectAssembly, TClassInMappingAssembly>()
        {
            var mappings = GetAutoPersistenceModelDefaults();

            mappings.AddEntityAssembly(Assembly.GetAssembly(typeof(TClassInDomainObjectAssembly))).Where(GetAutoMappingFilter);
            mappings.UseOverridesFromAssemblyOf<TClassInMappingAssembly>();

            return mappings;
        }

        private AutoPersistenceModel GetAutoPersistenceModelDefaults()
        {
            var mappings = new AutoPersistenceModel();
            mappings.Conventions.Setup(GetConventions());
            mappings.Setup(GetSetup());
            mappings.IgnoreBase<DomainObject>();
            mappings.IgnoreBase(typeof(DomainObjectWithTypedId<>));

            return mappings;
        }

        public virtual Action<IConventionFinder> GetConventions()
        {
            return c =>
            {
                AddPrimaryKeyConvention(c);
                AddHasManyConvention(c);
                AddManyToManyConvention(c);
                AddReferenceConvention(c);
                AddTableNameConvention(c);
            };
        }

        public virtual void AddPrimaryKeyConvention(IConventionFinder conventionFinder)
        {
            conventionFinder.Add<PrimaryKeyConvention>();
        }

        public virtual void AddHasManyConvention(IConventionFinder conventionFinder)
        {
            conventionFinder.Add<HasManyConvention>();
        }

        public virtual void AddManyToManyConvention(IConventionFinder conventionFinder)
        {
            conventionFinder.Add<ManyToManyConvention>();
        }

        public virtual void AddTableNameConvention(IConventionFinder conventionFinder)
        {
            conventionFinder.Add<TableNameConvention>();
        }

        public virtual void AddReferenceConvention(IConventionFinder conventionFinder)
        {
            conventionFinder.Add<ReferenceConvention>();
        }

        public virtual Action<AutoMappingExpressions> GetSetup()
        {
            return c =>
            {
                c.FindIdentity = type => type.Name == "Id";
            };
        }

        public virtual bool GetAutoMappingFilter(Type t)
        {
            return t.GetInterfaces().Any(x =>
                 x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));
        }
    }
}