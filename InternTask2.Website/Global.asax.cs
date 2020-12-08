using InternTask2.Website.Models;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace InternTask2.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            SharePointManager.CreateListNLibrary();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
