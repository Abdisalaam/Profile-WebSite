using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LeadersSolutionWebSite.Startup))]
namespace LeadersSolutionWebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
