using System.Text.Json.Serialization;

namespace JwtTokenMiddleware
{
    public class JwtTokenRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}