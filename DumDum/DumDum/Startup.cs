using DumDum.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using DumDum.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DumDum.Services;
using DumDum.Interfaces;
using DumDum.Interfaces.IRepositories;
using DumDum.Interfaces.IServices;
using DumDum.Repository;
using Serilog;
using Microsoft.OpenApi.Models;

namespace DumDum
{
    public class Startup
    {
        private IConfiguration AppConfig { get; }
        public Startup(IConfiguration configuration)
        {
            AppConfig = configuration;

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();

            services.AddControllersWithViews();
            services.AddTransient<IAuthenticateService, AuthenticateService>();
            services.AddTransient<IBattleService, BattleService>();
            services.AddTransient<IBuildingService, BuildingService>();
            services.AddTransient<IDetailService, DetailService>();
            services.AddTransient<IDumDumService, DumDumService>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IResourceService, ResourceService>();
            services.AddTransient<ITimeService, TimeService>();
            services.AddTransient<ITroopService, TroopService>();

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IBuildingRepository, BuildingRepository>();
            services.AddTransient<IKingdomRepository, KingdomRepository>();
            services.AddTransient<IPlayerRepository, PlayerRepository>();
            services.AddTransient<IResourceRepository, ResourceRepository>();
            services.AddTransient<ITroopLevelRepository, TroopLevelRepository>();
            services.AddTransient<ITroopRepository, TroopRepository>();
            services.AddTransient<ITroopTypesRepository, TroopTypesRepository>();
            services.AddTransient<IBattleRepository, BattleRepository>();
            services.AddTransient<ITroopsLostRepository, TroopsLostRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IHostedService, RecureHostedService>();

            ConfigureDb(services);

            //This is setting for authentication

            var appSettingSection = AppConfig.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingSection);

            //JWT Authentication...

            var appSettings = appSettingSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Key);

            services.AddAuthentication(au =>
            {
                au.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                au.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddControllers().AddNewtonsoftJson(options =>
                 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DumDum",
                    Description = ".NET 5 API App"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API V1");
                c.RoutePrefix = string.Empty;
            });


            app.UseSerilogRequestLogging();
        }

        private void ConfigureDb(IServiceCollection services)
        {
            var connectionString = AppConfig.GetConnectionString("DefaultConnection");
            var serverVersion = new MySqlServerVersion(new Version(8, 0));

            services.AddDbContext<ApplicationDbContext>(
                options => options
                .UseMySql(connectionString, serverVersion)
                // The following three options help with debugging
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
        }
    }
}
