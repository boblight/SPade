using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SPade.Startup))]
namespace SPade
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
