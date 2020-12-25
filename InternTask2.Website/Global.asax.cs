﻿using InternTask2.BLL;
using InternTask2.BLL.Services.Abstract;
using InternTask2.Website.Properties;
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

            NinjectModule bllModule = new BLLNinjectModule(
                "DefaultConnection",
                Settings.Default.SPSiteUrl,
                Settings.Default.SPDocLibName,
                Settings.Default.SPListName,
                Settings.Default.SPLogin,
                Settings.Default.SPPass
                );
            IKernel kernel = new StandardKernel(new AppNinjectModule(), bllModule);
            ISPService spInitializer = kernel.Get<ISPService>();
            spInitializer.Initialize();
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }
    }
}
