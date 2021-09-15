using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace JwtTokenMiddleware
{
    public abstract class JwtTokenHandle<TRequest, TRefreshRequest> : IJwtTokenHandle
        where TRequest : JwtTokenRequest
        where TRefreshRequest : JwtRefreshTokenRequest
    {
        protected abstract Task<Tuple<bool, List<Claim>>> TokenHandleValidateAsync(TRequest req);

        protected abstract Task<Tuple<bool, List<Claim>>> RefreshTokenHandleValidateAsync(TRefreshRequest req);

        protected abstract Task OnGenerateTokenAfterAsync(JwtTokenResponse jwtTokenResponse);

        public virtual async Task TokenHandleAsync(HttpContext context)
        {
            var req = await context.Request.ReadFromJsonAsync<TRequest>();
            var (result, claims) = await TokenHandleValidateAsync(req);
            if (!result)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await GenerateTokenAsync(context, claims);
        }

        public virtual async Task RefreshTokenHandleAsync(HttpContext context)
        {
            var req = await context.Request.ReadFromJsonAsync<TRefreshRequest>();
            var (result, claims) = await RefreshTokenHandleValidateAsync(req);
            if (!result)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await GenerateTokenAsync(context, claims);
        }

        protected virtual async Task GenerateTokenAsync(HttpContext context, List<Claim> customClaims)
        {
            var jwtTokenOptions = context.RequestServices.GetRequiredService<JwtTokenOptions>();

            var now = DateTime.Now;

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Exp,
                    $"{new DateTimeOffset(now.Add(jwtTokenOptions.ExpiresIn)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Iss, jwtTokenOptions.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, jwtTokenOptions.Audience),
            };
            claims.AddRange(customClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenOptions.SecurityKey));
            var cred = new SigningCredentials(key, jwtTokenOptions.Algorithm);

            var jwtSecurityToken = new JwtSecurityToken(
                signingCredentials: cred,
                claims: claims
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var jwtTokenResponse = new JwtTokenResponse(JwtBearerDefaults.AuthenticationScheme, jwtToken,
                (int)jwtTokenOptions.ExpiresIn.TotalSeconds, GenerateRefreshToken());

            await context.Response.WriteAsJsonAsync(jwtTokenResponse);

            await OnGenerateTokenAfterAsync(jwtTokenResponse);
        }

        protected virtual string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}