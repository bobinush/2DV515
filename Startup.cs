using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using mvc.Models;
// using NJsonSchema;
// using NSwag.AspNetCore;
using Z.EntityFramework.Extensions;

namespace mvc
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
            // services.Configure<CookiePolicyOptions>(options =>
            // {
            //     // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //     options.CheckConsentNeeded = context => true;
            //     options.MinimumSameSitePolicy = SameSiteMode.;
            // });

            string connString = Configuration.GetConnectionString("WI");
            services.AddDbContext<ApiDbContext>(options =>
                options.UseSqlite(connString));

            // Using a constructor that requires optionsBuilder (EF Core) 
            // To use EF bulkinsert (nuget package)
            EntityFrameworkManager.ContextFactory = context =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>();
                optionsBuilder.UseSqlServer(connString);
                return new ApiDbContext(optionsBuilder.Options);
            };

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // services.AddSwagger();
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            // app.UseCookiePolicy();

            // app.UseSwaggerUi3WithApiExplorer(settings =>
            //             {
            //                 settings.GeneratorSettings.DefaultPropertyNameHandling =
            //                 PropertyNameHandling.CamelCase;
            //                 settings.PostProcess = document =>
            //                 {
            //                     document.Info.Title = "2DV515 Web Intelligence";
            //                     document.Info.Description = @"Overview for the API created during the
            //         Web Intelligence course at Linnaeus University";
            //                     document.Info.Contact = new NSwag.SwaggerContact
            //                     {
            //                         Name = "Robin Nowakowski",
            //                         Email = string.Empty,
            //                         Url = "http://github.com/bobinush"
            //                     };
            //                 };
            //             });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
