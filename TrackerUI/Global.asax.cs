using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TrackerLibrary.Config;
using System.Web.Http;

namespace TrackerUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfig.InitializeConnection();

            AutoMapperConfig.RegisterMappings();

            var ninjectConfig = new NinjectConfig();
            DependencyResolver.SetResolver(ninjectConfig);
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(ninjectConfig.Kernel);
        }
    }
}
