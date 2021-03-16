using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace FeedsBL
{
    public class Startup
    {
        public readonly string AngularClientURL = "";
        public readonly bool ToUseProxyAngularClient;//= Configuration.GetValue<bool>("ToUseProxyAngularClient");
       // public readonly ILogger<Startup> Log;
        public Startup(IConfiguration configuration)//, ILogger<Startup> log)
        {
           
            Configuration = configuration;
            AngularClientURL = Configuration.GetValue<string>("AngularClientURL")
                                                ?? "http://localhost:4200";
            ToUseProxyAngularClient = Configuration.GetValue<bool>("ToUseProxyAngularClient");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                                        .WithOrigins(AngularClientURL)
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials());
                                    });
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
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication3 v1"));
                Console.WriteLine($"You may open Swagger  : {Program.ApplicationUrls[0]}/swagger ");
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseCors("CorsPolicy");
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
               // http://localhost:55000/swagger/index.html
                spa.Options.SourcePath = "ClientApp";
                spa.Options.DevServerPort = new Uri(AngularClientURL).Port;

                   //HACK
                if (!string.IsNullOrWhiteSpace(AngularClientURL) &&  env.IsDevelopment())
                {
                    if (ToUseProxyAngularClient)
                    {
                        Console.WriteLine($"Proxy to Spa  : {AngularClientURL} open it manualy");

                        spa.UseProxyToSpaDevelopmentServer(AngularClientURL);
                       }
                    else
                    {
                        Console.WriteLine($"Wait for translate and open ClientApp : {AngularClientURL} in default browser");
                        spa.UseAngularCliServer(npmScript: "start");
                     }

        

                }
            });
        }
    }
}
