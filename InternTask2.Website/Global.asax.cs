using InternTask2.Website.Models;
using InternTask2.Website.Services.Abstract;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Mvc;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace InternTask2.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            IKernel kernel = new StandardKernel(new AppNinjectModule());
            ISharePointManager sharePointManager = kernel.Get<ISharePointManager>();
            sharePointManager.CreateListNLibrary();
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }
    }
}
