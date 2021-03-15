using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text.Json;

namespace FeedsBL
{
    public class Startup
    {
        public readonly string AngularClienURL = "";
        public readonly bool ToUseProxyAngularClient;//= Configuration.GetValue<bool>("ToUseProxyAngularClient");



        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AngularClienURL = Configuration.GetValue<string>("AngularClienURL")
                                                ?? "http://localhost:4200";
            ToUseProxyAngularClient = Configuration.GetValue<bool>("ToUseProxyAngularClient");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDataService,DataService>();

            services.AddControllersWithViews()
                .AddJsonOptions(option =>
                option.JsonSerializerOptions.PropertyNamingPolicy
                = JsonNamingPolicy.CamelCase);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AndreyCurrencyBL", Version = "v1" });
            });

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

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication3 v1"));

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";
                //HACK
                if (false && !string.IsNullOrWhiteSpace(AngularClienURL) &&  env.IsDevelopment())
                {
                    if ( ToUseProxyAngularClient)
                    {
                        spa.UseProxyToSpaDevelopmentServer(AngularClienURL);
                    }
                    else
                    {
                        spa.UseAngularCliServer(npmScript: "start");
                    }
                }
            });
        }
    }
}
