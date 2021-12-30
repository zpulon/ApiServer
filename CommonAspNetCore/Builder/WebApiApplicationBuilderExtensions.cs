using CommonAspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonAspNetCore.Builder
{
    public static class WebApiApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebApi(this IApplicationBuilder app, params object[] args)
        {
            app = app.UseMiddleware<HttpRequestLogScopeMiddleware>(Array.Empty<object>());
            return app;
        }
    }
}
