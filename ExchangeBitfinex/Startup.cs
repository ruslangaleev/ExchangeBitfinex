using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ExchangeBitfinex.Data;
using ExchangeBitfinex.Models;
using System.Reflection;
using System;
using ExchangeBitfinex.Data.Infrastructure;
using ExchangeBitfinex.Services.Services;
using ExchangeBitfinex.Data.Repositories;
using Microsoft.IdentityModel.Tokens;
using ExchangeBitfinex.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

namespace ExchangeBitfinex
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Интерфейс для получения данных окружения
        /// </summary>
        private readonly IHostingEnvironment _env;
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var dbConnectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(dbConnectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        });
                },
                ServiceLifetime.Scoped
            );

            var authOptions = Configuration.GetSection("AuthOptions").Get<AuthOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // укзывает, будет ли валидироваться издатель при валидации токена 
                            ValidateIssuer = true,
                            // строка, представляющая издателя 
                            ValidIssuer = authOptions.Issuer,

                            // будет ли валидироваться потребитель токена 
                            ValidateAudience = true,
                            // установка потребителя токена 
                            ValidAudience = authOptions.Audience,
                            // будет ли валидироваться время существования 
                            ValidateLifetime = true,

                            // установка ключа безопасности 
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authOptions.Key)),
                            // валидация ключа безопасности 
                            ValidateIssuerSigningKey = true
                        };
                    });

            services.AddSwaggerGen(options =>
            {
                var xmlPath = Path.Combine(_env.ContentRootPath, "ExchangeBitfinex.xml");
                options.IncludeXmlComments(xmlPath);

                options.SwaggerDoc("v1", new Info
                {
                    Title = "ReportOnline Dispatcher",
                    Version = "v1",
                    Description = "",
                    TermsOfService = ""
                });

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "Пример: Bearer {токен}",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
            });

            services.AddScoped<IStorageContext, ApplicationDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICurrencyInfoManager, CurrencyInfoManager>();
            services.AddSingleton<IBitfinexClient, BitfinexClient>();
            services.AddScoped<ICurrencyInfoRepository, CurrencyInfoRepository>();
            services.AddSingleton<IBitfinexHandler, BitfinexHandler>();
            services.Configure<AuthOptions>(Configuration.GetSection("AuthOptions"));

            services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddAuthorization()
                .AddDataAnnotations()
                .AddApiExplorer();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var basePath = Configuration.GetValue<string>("BasePath") ?? "";
            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.RoutePrefix = "api-docs";
                   c.SwaggerEndpoint($"{basePath}/swagger/v1/swagger.json", "Тестовое задание");
               });

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            var bitfinexHandler = app.ApplicationServices.GetRequiredService<IBitfinexHandler>();
            bitfinexHandler.Start();
        }
    }
}
