using Microsoft.Owin;
using Owin;
using Newtonsoft.Json;
using Hangfire;
using Hangfire.SqlServer;
using SPade.Models.DAL;
using SPade.Filter;

[assembly: OwinStartupAttribute(typeof(SPade.Startup))]
namespace SPade
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            SPadeDBEntities db = new SPadeDBEntities();
            GlobalConfiguration.Configuration.UseSqlServerStorage(db.Database.Connection.ConnectionString);
            var options = new DashboardOptions
            {
                AppPath = "/Admin/Dashboard",
                Authorization = new[]
             {
                 new HangfireAuthorizationFilter()
             }
            };
            app.UseHangfireDashboard("/HangfireDashboard", options);
            app.UseHangfireServer();
        }
    }
}
