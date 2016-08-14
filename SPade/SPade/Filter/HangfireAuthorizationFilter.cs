using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Dashboard;
using Hangfire.Annotations;

namespace SPade.Filter
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            bool isAuthenticated = false;

            if (HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User.IsInRole("Admin"))
            {
                isAuthenticated = true;
            }

            return isAuthenticated;
        }
    }
}