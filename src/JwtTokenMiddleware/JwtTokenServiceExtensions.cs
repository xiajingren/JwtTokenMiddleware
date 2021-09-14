using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace JwtTokenMiddleware
{
    public static class JwtTokenServiceExtensions
    {
        public static IServiceCollection AddJwtTokenAuthentication(
            this IServiceCollection services,
            Action<JwtTokenOptions> configureJwtTokenOptions,
            Action<JwtBearerOptions> configureJwtBearerOptions = null)
        {
            var jwtTokenOptions = new JwtTokenOptions();
            configureJwtTokenOptions.Invoke(jwtTokenOptions);

            if (string.IsNullOrEmpty(jwtTokenOptions.SecurityKey))
            {
                throw new ArgumentException($"{nameof(jwtTokenOptions.SecurityKey)} is required");
            }

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(configureJwtBearerOptions ?? (options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtTokenOptions.Issuer,
                        ValidAudience = jwtTokenOptions.Audience,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenOptions.SecurityKey)),
                        ValidateIssuerSigningKey = true,
                    };
                }));

            services.AddSingleton(jwtTokenOptions);

            return services;
        }

    }
}