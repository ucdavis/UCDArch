using Castle.Windsor;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.NHibernateValidator.CommonValidatorAdapter;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using SampleUCDArchApp.Core;

namespace SampleUCDArchApp
{
    internal static class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddGenericRepositoriesTo(container);

            container.AddComponent("messageFactory", 
                                    typeof (IMessageFactory), 
                                    typeof (MessageFactory));

            container.AddComponent("validator",
                                   typeof(IValidator), typeof(Validator));
            container.AddComponent("dbContext", typeof (IDbContext), typeof (DbContext));

        }

        private static void AddGenericRepositoriesTo(IWindsorContainer container)
        {
            container.AddComponent("repositoryWithTypedId",
                typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));
            container.AddComponent("repositoryType",
                                   typeof(IRepository<>), typeof(Repository<>));
            container.AddComponent("repository",
                                   typeof(IRepository), typeof(Repository));
            
        }
    }
}