using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Alebob.Training.Converters;
using Alebob.Training.DataLayer;
using Microsoft.Extensions.Options;
using Alebob.Training.DataLayer.Services;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Alebob.Training
{
    public class Startup
    {
        private string frontIndexUrl;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            frontIndexUrl = configuration.GetValue<string>("Application:frontAddress");
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
                    options.EventsType = typeof(OAuth.GoogleAuthEventsHandler);
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
                            Uri originUri = new Uri(origin),
                                frontUri = new Uri(frontIndexUrl);
                            return Uri.Compare(originUri, frontUri,
                                UriComponents.HostAndPort,
                                UriFormat.UriEscaped,
                                StringComparison.OrdinalIgnoreCase) == 0;
                        })
                        .AllowAnyHeader()
                        .WithExposedHeaders("location")
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));
            services.AddSingleton<OAuth.GoogleAuthEventsHandler>();
            services.AddSingleton(typeof(IHistoryProvider), typeof(HistoryService));
            services.AddSingleton(typeof(IUserProvider), typeof(UsersService));
            services.AddSingleton(typeof(IExerciseProvider), typeof(ExercisesService));
            services.AddSingleton<IDatabaseSettings>(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            services.AddSingleton<HistoryService>();
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
                endpoints.MapFallbackToController("Get", "Index");
            });

            if (!env.IsDevelopment())
            {
                app.UseDefaultFiles(new DefaultFilesOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Content")),
                    RequestPath = ""
                });
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Content")),
                    RequestPath = "",
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age=600");
                    }
                });
            }
        }
    }
}
