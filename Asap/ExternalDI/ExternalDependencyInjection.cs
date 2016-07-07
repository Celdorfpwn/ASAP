using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationDI;
using GitService;
using IssuesTracking;
using JiraService;
using Microsoft.Practices.Unity;
using SourceControl;
using ToolsConfiguration;
using CodeReview;
using FishEyeService;

namespace ExternalDI
{
    public class ExternalDependencyInjection
    {
        private static IUnityContainer _container;

        static ExternalDependencyInjection()
        {
            _container = new UnityContainer();

            ConfigureDIs();
        }

        private static void ConfigureDIs()
        {
            var configuration = ConfigurationDependencyInjection.Resolve<IConfiguration>();
            _container.RegisterType<ISourceControl, Git>(new InjectionConstructor(configuration.SourceControlConfig));
            _container.RegisterType<IIssuesTracking, Jira>(new InjectionConstructor(configuration.IssuesTrackingConfig));
            _container.RegisterType<ICodeReview, FishEye>(new InjectionConstructor(configuration.CodeReviewConfig));
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
