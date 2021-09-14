using System.Text.Json.Serialization;

namespace JwtTokenMiddleware
{
    public class JwtTokenResponse
    {
        public JwtTokenResponse(string tokenType, string accessToken, int expiresIn, string refreshToken)
        {
            TokenType = tokenType;
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
        }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}