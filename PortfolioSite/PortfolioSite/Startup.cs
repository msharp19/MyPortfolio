using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PortfolioSite.Startup))]
namespace PortfolioSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
