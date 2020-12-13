using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChatServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ChatServices.SignalRHubs;
using ChatServices.Providers;
using Microsoft.EntityFrameworkCore;


namespace ChatServices
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

            services.AddTransient<ValidateMimeMultipartContentFilter>();

            var sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<NewsContext>(options =>
                options.UseSqlite(
                    sqlConnectionString
                ), ServiceLifetime.Singleton
            );

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(
                        "http://localhost")
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod();
                });
            });

            services.AddSingleton<NewsStore>();

            services.AddSignalR();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCors();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<LoopyHub>("/loopy");
                endpoints.MapHub<NewsHub>("/looney");
                endpoints.MapHub<LoopyMessageHub>("/loopymessage");
                endpoints.MapHub<ImagesMessageHub>("/zub");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
