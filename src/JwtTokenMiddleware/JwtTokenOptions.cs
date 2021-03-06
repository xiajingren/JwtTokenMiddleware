using System;
using Microsoft.IdentityModel.Tokens;

namespace JwtTokenMiddleware
{
    public class JwtTokenOptions
    {
        /// <summary>
        /// default:    "/auth/token"
        /// </summary>
        public string TokenPath { get; set; } = "/auth/token";

        /// <summary>
        /// default:    "/auth/refresh_token"
        /// </summary>
        public string RefreshTokenPath { get; set; } = "/auth/refresh_token";

        /// <summary>
        /// default:    "JwtToken_Issuer"
        /// </summary>
        public string Issuer { get; set; } = "JwtToken_Issuer";

        /// <summary>
        /// default:    "JwtToken_Audience"
        /// </summary>
        public string Audience { get; set; } = "JwtToken_Audience";

        /// <summary>
        /// is required
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// default:    10min
        /// </summary>
        public TimeSpan ExpiresIn { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// default: SecurityAlgorithms.HmacSha256
        /// </summary>
        public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;

        public Type HandleType { get; private set; }

        /// <summary>
        /// please inherit from <see cref="JwtTokenHandle{TRequest,TRefreshRequest}"/>
        /// </summary>
        /// <typeparam name="THandle"></typeparam>
        public void RegisterHandle<THandle>() where THandle : IJwtTokenHandle
        {
            HandleType = typeof(THandle);
        }

    }
}