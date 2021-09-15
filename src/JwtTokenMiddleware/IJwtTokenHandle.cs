using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JwtTokenMiddleware
{
    public interface IJwtTokenHandle
    {
        Task TokenHandleAsync(HttpContext context);

        Task RefreshTokenHandleAsync(HttpContext context);
    }
}