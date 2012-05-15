using System;
using System.Collections.Generic;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg;
using NHibernate.Stat;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace UCDArch.Testing
{
    public abstract class RepositoryTestBase
    {
        public List<Guid> UserIds { get; set; }
        public IRepository Repository { get; set; }

        private static readonly Configuration Configuration = new Configuration().Configure();

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

        protected RepositoryTestBase()
        {
            UserIds = new List<Guid>();
            Repository = new Repository();
        }

        /// <summary>
        /// Creates the DB.
        /// </summary>
        [TestInitialize]
        public void CreateDB()
        {
            new NHibernate.Tool.hbm2ddl.SchemaExport(Configuration).Execute(false, true, false,
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