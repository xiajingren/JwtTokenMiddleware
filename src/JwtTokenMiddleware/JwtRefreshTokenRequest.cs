using System.Text.Json.Serialization;

namespace JwtTokenMiddleware
{
    public class JwtRefreshTokenRequest
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}