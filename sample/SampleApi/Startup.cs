using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtTokenMiddleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

            services.AddScoped<MyJwtTokenRequest>();
            services.AddJwtTokenAuthentication(options =>
            {
                options.SecurityKey = @"qwd@efw3ef#7regre$5trhhy%juj";
                options.RegisterHandle<MyHandle>();
            });
        }

        public class MyHandle : JwtTokenHandle<MyJwtTokenRequest, MyJwtRefreshTokenRequest>
        {
            protected override async Task<Tuple<bool, List<Claim>>> TokenHandleValidateAsync(MyJwtTokenRequest req)
            {
                await Task.CompletedTask;

                var claims = new List<Claim> { new Claim("test", "111") };

                var result = req.Username == "admin" && req.Password == "123456";

                return new Tuple<bool, List<Claim>>(result, claims);
            }

            protected override async Task<Tuple<bool, List<Claim>>> RefreshTokenHandleValidateAsync(MyJwtRefreshTokenRequest req)
            {
                await Task.CompletedTask;

                var claims = new List<Claim> { new Claim("test", "111") };

                var result = !string.IsNullOrEmpty(req.RefreshToken);

                return new Tuple<bool, List<Claim>>(result, claims);
            }

            protected override async Task OnGenerateTokenAfterAsync(JwtTokenResponse jwtTokenResponse)
            {
                //todo:
                await Task.CompletedTask;
            }
        }

        public class MyJwtTokenRequest : JwtTokenRequest
        {

        }

        public class MyJwtRefreshTokenRequest : JwtRefreshTokenRequest
        {

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

            app.UseJwtToken();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}