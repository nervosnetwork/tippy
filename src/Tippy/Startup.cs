using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tippy.Filters;
using Tippy.Hubs;

namespace Tippy
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
            services.AddSignalR();

            var mvcBuilder = services.AddRazorPages()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                })
                .AddMvcOptions(options =>
                {
                    options.Filters.Add(new PageMessageFilter());
                });
#if RAZOR_RUNTIMECOMPILATIION
            mvcBuilder.AddRazorRuntimeCompilation();
#endif

            services.AddScoped<ActiveProjectFilter>();

            services.AddDbContext<Core.Data.TippyDbContext>(options =>
            {
                var dbPath = Path.Combine(Core.Environment.GetAppDataFolder(), "tippy-db.db");
                options.UseSqlite($"Data Source={dbPath}");
            });
            services.AddDatabaseDeveloperPageExceptionFilter();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStatusCodePagesWithReExecute("/NotFound", "?statusCode={0}");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapControllers();
                endpoints.MapRazorPages();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapHub<LogHub>("/loghub");

                // endpoints.MapFallbackToController("Index", "Home");
            });
        }
    }
}
