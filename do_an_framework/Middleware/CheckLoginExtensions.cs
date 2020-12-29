using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace do_an_framework.Middleware
{
    public static class CheckLoginExtensions
    {
        public static IApplicationBuilder UseCheckLoginAccess(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckLoginMiddleware>();
        }
    }
}
