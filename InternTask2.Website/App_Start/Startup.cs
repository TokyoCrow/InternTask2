using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(InternTask2.Website.App_Start.Startup))]

namespace InternTask2.Website.App_Start
{
    public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = "ApplicationCookie",
                    LoginPath = new PathString("/Account/Login"),
                    LogoutPath = new PathString("/Account/Logout")
                });
            }
        }
}