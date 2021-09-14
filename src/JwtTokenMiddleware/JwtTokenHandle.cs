using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace JwtTokenMiddleware
{
    public abstract class JwtTokenHandle<TRequest> where TRequest : IJwtTokenRequest
    {
        private readonly JwtTokenOptions _jwtTokenOptions;

        protected JwtTokenHandle(JwtTokenOptions jwtTokenOptions)
        {
            _jwtTokenOptions = jwtTokenOptions;
        }

        public abstract Task<bool> HandleAsync(TRequest req);

        public virtual async Task<JwtTokenResponse> GenerateToken(TRequest req)
        {
            if (await HandleAsync(req))
            {
                return null;
            }

            var now = DateTime.Now;

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(now).ToUnixTimeSeconds()}") ,
                new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(now.Add(_jwtTokenOptions.ExpiresIn)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Iss,_jwtTokenOptions.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud,_jwtTokenOptions.Audience),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenOptions.SecurityKey));
            var cred = new SigningCredentials(key, _jwtTokenOptions.Algorithm);

            var jwtSecurityToken = new JwtSecurityToken(
                signingCredentials: cred,
                claims: claims
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var jwtTokenResponse = new JwtTokenResponse(JwtBearerDefaults.AuthenticationScheme, jwtToken,
                (int)_jwtTokenOptions.ExpiresIn.TotalMilliseconds, GenerateRefreshToken());

            return jwtTokenResponse;
        }

        public virtual string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}