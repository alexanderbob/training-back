using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Text.Json;
using Alebob.Training.Converters;
using Alebob.Training.DataLayer;
using System.Net;

namespace Alebob.Training
{
    public class Startup
    {
        private string frontIndexUrl;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            IConfigurationSection section = Configuration.GetSection("Application");
            frontIndexUrl = section["frontAddress"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                        Configuration.GetSection("Google");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    options.Events.OnRedirectToAuthorizationEndpoint = (context) =>
                    {
                        context.Response.Headers["location"] = context.RedirectUri;
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return Task.CompletedTask;
                    };
                    options.Events.OnCreatingTicket = (context) =>
                    {
                        context.Properties.RedirectUri = frontIndexUrl;
                        return Task.CompletedTask;
                    };
                });
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
#if DEBUG
                    options.JsonSerializerOptions.WriteIndented = true;
#endif
                });
                
            services.AddCors(setup =>
            {
                setup.AddDefaultPolicy(builder =>
                {
                    builder
                        .SetIsOriginAllowed(origin => {
                            return origin.Contains(frontIndexUrl, StringComparison.OrdinalIgnoreCase);
                        })
                        .AllowAnyHeader()
                        .WithExposedHeaders("location")
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddSingleton(typeof(IHistoryProvider), typeof(HistoryProvider));
            services.AddSingleton(typeof(IExerciseProvider), typeof(ExerciseProvider));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors();
            app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
