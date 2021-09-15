using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using JwtTokenMiddleware;

namespace SampleApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddJwtTokenAuthentication(options =>
            {
                // options.Issuer = "my_iss";
                // options.Audience = "my_iss";
                // options.ExpiresIn = TimeSpan.FromMinutes(30);
                // options.TokenPath = "/token";
                // options.RefreshTokenPath = "/refresh_token";
                options.SecurityKey = @"B3SACXE7shay8+mNe59qO987DM94+YOrzNtUex5I2UI=";
                options.RegisterHandle<MyHandle>();
            });
        }

        public class MyHandle : JwtTokenHandle<MyJwtTokenRequest, MyJwtRefreshTokenRequest>
        {
            protected override async Task<Tuple<bool, List<Claim>>> TokenHandleValidateAsync(MyJwtTokenRequest req)
            {
                await Task.CompletedTask;

                // validate logic ...
                var result = req.Username == "admin" && req.Password == "123456";
                if (!result)
                    return new Tuple<bool, List<Claim>>(false, null);

                var claims = new List<Claim> {new Claim("my_claim", "my_claim_value")};
                return new Tuple<bool, List<Claim>>(true, claims);
            }

            protected override async Task<Tuple<bool, List<Claim>>> RefreshTokenHandleValidateAsync(
                MyJwtRefreshTokenRequest req)
            {
                await Task.CompletedTask;

                // validate logic ...
                var result = req.RefreshToken == "z849OH9T+opbUXm8vVD1b2/M87KzClfv4YFEQwhZAYo=";
                if (!result)
                    return new Tuple<bool, List<Claim>>(false, null);

                var claims = new List<Claim> {new Claim("my_claim", "my_claim_value")};
                return new Tuple<bool, List<Claim>>(true, claims);
            }

            protected override async Task OnGenerateTokenAfterAsync(JwtTokenResponse jwtTokenResponse)
            {
                await Task.CompletedTask;

                // save the refresh_token to database ...
            }
        }

        public class MyJwtTokenRequest : JwtTokenRequest
        {
            // custom parameters
        }

        public class MyJwtRefreshTokenRequest : JwtRefreshTokenRequest
        {
            // custom parameters
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseJwtToken(); // add this line

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}