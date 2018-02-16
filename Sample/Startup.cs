using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Identity.MongoDB;
using AspNetCore.MongoDB;
using Sample.Models;
using Microsoft.AspNetCore.Identity;
using AspNetCore.MongoDB.JWTIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Sample
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
            services.AddIdentity<ApplicationUser, MongoIdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            }).AddDefaultTokenProviders();

            services
                .Configure<MongoDBOption>(Configuration.GetSection("MongoDBOption"))
                .Configure<JWTSettings>(Configuration.GetSection("JWTSettings"))
                .AddMongoDatabase()
                .AddJWTIdentity<ApplicationUser, MongoIdentityRole>(Configuration.GetSection("JWTSettings"));

            

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();


            app.UseMvc();
        }
    }
}
