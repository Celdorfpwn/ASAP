using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BL;
using ExternalDI;
using IssuesTracking;
using Microsoft.Practices.Unity;
using SourceControl;
using CodeReview;
using DatabaseDI;
using Repository;


namespace ModelsDI
{
    public static class ModelsDependencyInjection
    {

        private static IUnityContainer _container;

        static ModelsDependencyInjection()
        {
            _container = new UnityContainer();

            ConfigureDIs();
        }

        private static void ConfigureDIs()
        {

            _container.RegisterType<TasksModel>(new InjectionConstructor(ExternalDependencyInjection.Resolve<ISourceControl>()
                , ExternalDependencyInjection.Resolve<IIssuesTracking>()
                , DatabaseDependencyInjection.Resolve<IRepositoryFactory>()));

            _container.RegisterType<IssuesTrackingModel>(new InjectionConstructor(ExternalDependencyInjection.Resolve<ISourceControl>() 
                , ExternalDependencyInjection.Resolve<IIssuesTracking>()
                , DatabaseDependencyInjection.Resolve<IRepositoryFactory>()));

            _container.RegisterType<CodeReviewModel>(new InjectionConstructor(ExternalDependencyInjection.Resolve<IIssuesTracking>()
                , ExternalDependencyInjection.Resolve<ICodeReview>()
                , DatabaseDependencyInjection.Resolve<IRepositoryFactory>()));
        }

        public static Dependency Resolve<Dependency>()
        {
            Dependency instance = default(Dependency);

            if (_container.IsRegistered(typeof(Dependency)))
            {
                instance = _container.Resolve<Dependency>();
            }

            return instance;
        }
    }
}
