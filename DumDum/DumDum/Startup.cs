using DumDum.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DumDum.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DumDum.Services;
using DumDum.Interfaces;
using DumDum.Repository;

namespace DumDum
{
    public class Startup
    {
        private IConfiguration AppConfig { get; }
        public Startup(IConfiguration configuration)
        {
            AppConfig = configuration;
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
                //c.RoutePrefix = string.Empty;
            });
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
