using Microsoft.AspNetCore.Builder;

namespace JwtTokenMiddleware
{
    public static class JwtTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtTokenMiddleware>();
        }
    }
}