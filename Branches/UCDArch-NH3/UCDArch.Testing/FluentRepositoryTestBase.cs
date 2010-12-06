using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Windsor;
using FluentNHibernate.Cfg;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg;
using NHibernate.Stat;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Data.NHibernate.Fluent;

namespace UCDArch.Testing
{
    public abstract class FluentRepositoryTestBase<TMappingClass> : FluentRepositoryTestBase<TMappingClass, PrimaryKeyConvention> {}
    
    public abstract class FluentRepositoryTestBase<TMappingClass, TConventionClass>
    {
        public List<Guid> UserIds { get; set; }
        public IRepository Repository { get; set; }

        private readonly Assembly _mappingAssembly;
        private readonly Assembly _conventionAssembly;

        /// <summary>
        /// Get the statistics for the associated session factory
        /// </summary>
        /// <remarks>
        /// Must have <property name="generate_statistics">true</property>
        /// </remarks>
        /// <see cref="http://nhforge.org/blogs/nhibernate/archive/2008/10/26/exploring-nhibernate-statistics-part-1-simple-data-fetching.aspx"/>
        public IStatistics FactoryStatistics
        {
            get
            {
                return NHibernateSessionManager.Instance.FactoryStatistics;
            }
        }

        protected FluentRepositoryTestBase()
        {
            UserIds = new List<Guid>();
            Repository = new Repository();

            _mappingAssembly = typeof (TMappingClass).Assembly;
            _conventionAssembly = typeof (TConventionClass).Assembly;

            NHibernateSessionConfiguration.Mappings.UseFluentMappings(_mappingAssembly, _conventionAssembly);
        }

        /// <summary>
        /// Creates the DB.
        /// </summary>
        [TestInitialize]
        public void CreateDB()
        {
            var config = Fluently
                .Configure()
                .Mappings(x => x.FluentMappings.AddFromAssembly(_mappingAssembly)
                       .Conventions.AddAssembly(_conventionAssembly))
                .BuildConfiguration();
            
            new NHibernate.Tool.hbm2ddl.SchemaExport(config).Execute(false, true, false,
                                                                     NHibernateSessionManager.
                                                                         Instance.GetSession().Connection, null);
            InitServiceLocator();

            LoadData();
        }

        protected virtual void LoadData()
        {

        }

        protected virtual void InitServiceLocator()
        {
            var container = ServiceLocatorInitializer.Init();

            RegisterAdditionalServices(container);
        }

        /// <summary>
        /// Instead of overriding InitServiceLocator, you can jump in here to register additional services for testing
        /// </summary>
        protected virtual void RegisterAdditionalServices(IWindsorContainer container)
        {
            
        }
    }
}
