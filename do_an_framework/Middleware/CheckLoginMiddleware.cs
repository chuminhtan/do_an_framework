using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace do_an_framework.Middleware
{
    public class CheckLoginMiddleware
    {
        private readonly RequestDelegate _next;
        public CheckLoginMiddleware(RequestDelegate next) => _next = next;
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/admin/categories/list") 
            {
                string? user_id = httpContext.Session.GetString("user_id");
                if (user_id == null && httpContext.Request.Path.Value != "/admin/Admin/Login")
                {
                    httpContext.Response.Redirect("/Admin/Login");
                }
            } 
            else
            {
                await _next(httpContext);
            }
            
        }
    }
}
