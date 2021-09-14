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

        public class MyHandle : JwtTokenHandle<MyJwtTokenRequest>
        {
            protected override async Task<bool> OnGenerateTokenBeforeAsync(MyJwtTokenRequest req, List<Claim> claims)
            {
                await Task.CompletedTask;

                return req.Username == "admin" && req.Password == "123456";
            }

            protected override async Task OnGenerateTokenAfterAsync(JwtTokenResponse jwtTokenResponse)
            {
                //todo:
                await Task.CompletedTask;
            }
        }

        public class MyJwtTokenRequest : IJwtTokenRequest
        {
            public string Username { get; set; }

            public string Password { get; set; }
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