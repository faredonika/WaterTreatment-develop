using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WaterTreatment.Web.Services;

namespace WaterTreatment.Web
{

    public class MvcApplication : System.Web.HttpApplication
    {

        Container MasterContainer;

        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            MasterContainer = CompositionRoot.Build();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(MasterContainer));

            PeriodicEmails();
        }

        private void PeriodicEmails()
        {

            var obs = Observable.Interval(TimeSpan.FromMinutes(1));

            obs.Subscribe(x => {

                new EmailService(new WTContext()).SendRegisteredEmails();

            });

        }

    }

}
