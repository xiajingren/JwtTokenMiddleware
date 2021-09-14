using System;

namespace JwtTokenMiddleware
{
    public class JwtTokenOptions
    {
        /// <summary>
        /// get token path
        /// default:    "/token"
        /// </summary>
        public string TokenPath { get; set; } = "/auth/token";

        /// <summary>
        /// refresh token path
        /// default:    "/refresh_token"
        /// </summary>
        public string RefreshTokenPath { get; set; } = "/auth/refresh_token";

        /// <summary>
        /// issuer
        /// default:    "JwtToken_Issuer"
        /// </summary>
        public string Issuer { get; set; } = "JwtToken_Issuer";

        /// <summary>
        /// issuer
        /// default:    "JwtToken_Audience"
        /// </summary>
        public string Audience { get; set; } = "JwtToken_Audience";

        /// <summary>
        /// security key
        /// required
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// expire in
        /// default:    10min
        /// </summary>
        public TimeSpan ExpireIn { get; set; } = TimeSpan.FromMinutes(10);

    }
}