using AndreyCurrecyBL.Services;
using AndreyCurrencyBL.HubConfig;
using AndreyCurrencyBL.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text.Json;

namespace AndreyCurrencyBL
{
    /// <summary>
    /// class central business logic for system
    /// </summary>
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

            services.AddHttpClient();
            string angularClienURL = //61000
                Configuration.GetValue<string>("AngularClienURL")
                ?? "http://localhost:4200";

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                .WithOrigins(angularClienURL)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });

            services.AddSignalR();// SignalR

            services.AddSingleton<ICurrencyRatiosHTTPConsumer,CurrencyRatiosHTTPConsumer>();


           // services.AddTransient<IRatioEnentsHubFacade,RatioEnentsHubFacade>();
            /// <summary>
            /// Here may be implemented service factory to consume 
            /// Ratios from variois services
            /// </summary>

            services.AddSingleton<ICentralBLService, CentralBLService>();

            services.AddControllersWithViews().AddJsonOptions( option=>
                option.JsonSerializerOptions.PropertyNamingPolicy 
                = JsonNamingPolicy.CamelCase
                );
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
                endpoints.MapControllers();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<RartioHub>("/api/signalr");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    string AngularClienURL = Configuration.GetValue<string>("AngularClienURL") ?? "";
                    bool toUseProxyAngularClient = Configuration.GetValue<bool>("ToUseProxyAngularClient");
                    if (!string.IsNullOrWhiteSpace(AngularClienURL))
                    {
                        if (toUseProxyAngularClient)
                        {
                            spa.UseProxyToSpaDevelopmentServer(AngularClienURL);
                        }
                        else
                        {
                            spa.UseAngularCliServer(npmScript: "start");
                        }

                    }

                }
            });

        }
    }
}
