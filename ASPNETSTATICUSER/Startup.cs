using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPNETSTATICUSER.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASPNETSTATICUSER
{
    public class Startup
    {
        public Startup(IConfiguration configuration,IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUserService>(new UserService(this.getSystemUser()));

            services.AddAuthentication(configureOptions =>
            {
                configureOptions.DefaultAuthenticateScheme= CookieAuthenticationDefaults.AuthenticationScheme;
                configureOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                configureOptions.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options => {
                options.LoginPath = "/auth/signin";

                // SET SECURE FLAG ON COOKIE
                options.Cookie.SecurePolicy = HostingEnvironment.IsDevelopment()? CookieSecurePolicy.None : CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;

                options.ExpireTimeSpan = TimeSpan.FromHours(1);

            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseExceptionHandler("/Error");
                
            }

            app.UseStaticFiles();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "Default",
                template: "{controller=Dashboard}/{action=Index}");
            });
        }

        public Dictionary<string, string> getSystemUser()
        {
            return new Dictionary<string, string>()
            {
                {"admin","admin"}
            };
        }
    }

    
}
