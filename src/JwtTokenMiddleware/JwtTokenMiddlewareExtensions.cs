using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JwtTokenMiddleware
{
    public static class JwtTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtToken(this IApplicationBuilder builder)
        {
            //JwtTokenOptions jwtTokenOptions;
            //using (var scope = builder.ApplicationServices.CreateScope())
            //{
            //    jwtTokenOptions = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<JwtTokenOptions>>().Value;
            //}

            //return builder.UseMiddleware<JwtTokenMiddleware>(jwtTokenOptions);

            return builder.UseMiddleware<JwtTokenMiddleware>();
        }
    }
}