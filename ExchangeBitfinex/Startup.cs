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

namespace ExchangeBitfinex
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
                    //.AddAuthentication(o =>
                    //{
                    //    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    //    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    //    o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    //})
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

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.AddScoped<IStorageContext, ApplicationDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICurrencyInfoManager, CurrencyInfoManager>();
            services.AddScoped<IBitfinexClient, BitfinexClient>();
            services.AddScoped<ICurrencyInfoRepository, CurrencyInfoRepository>();
            services.Configure<AuthOptions>(Configuration.GetSection("AuthOptions"));

            services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddAuthorization();
                //.AddApiExplorer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            new BitfinexHandler(Configuration,
                serviceProvider.GetRequiredService<IBitfinexClient>(),
                serviceProvider.GetRequiredService<ICurrencyInfoManager>()).Start();
        }
    }
}
