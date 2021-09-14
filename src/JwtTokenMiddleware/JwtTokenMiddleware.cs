using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace JwtTokenMiddleware
{
    public class JwtTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtTokenOptions _jwtTokenOptions;

        public JwtTokenMiddleware(RequestDelegate next, JwtTokenOptions jwtTokenOptions)
        {
            _next = next;
            _jwtTokenOptions = jwtTokenOptions;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var httpMethod = context.Request.Method;
            var path = context.Request.Path.Value;

            if (string.IsNullOrEmpty(path))
            {
                await _next(context);
                return;
            }

            // token path
            if (httpMethod == HttpMethods.Get &&
                Regex.IsMatch(path, $"^/?{_jwtTokenOptions.TokenPath}/?$", RegexOptions.IgnoreCase))
            {
                await context.Response.WriteAsJsonAsync(new { Token = "token..." });
                return;
            }

            // refresh token path
            if (httpMethod == HttpMethods.Get &&
                Regex.IsMatch(path, $"^/?{_jwtTokenOptions.RefreshTokenPath}/?$", RegexOptions.IgnoreCase))
            {
                await context.Response.WriteAsJsonAsync(new { Token = "refresh token..." });
                return;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
