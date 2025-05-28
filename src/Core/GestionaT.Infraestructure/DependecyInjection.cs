using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using GestionaT.Persistence.PGSQL;
using GestionaT.Persistence.Common;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Infraestructure.Auth;
using GestionaT.Shared.Abstractions;
using GestionaT.Application.Interfaces.Reports;
using GestionaT.Infraestructure.Reports;
using GestionaT.Application.Interfaces.Images;
using GestionaT.Infraestructure.Common.Images;
using GestionaT.Infraestructure.Images;
using GestionaT.Infraestructure.Auth.OAuths;

namespace GestionaT.Infraestructure
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfraestructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AppPostgreSqlDbContext>()
            .AddDefaultTokenProviders();

            var jwtSettings = configuration.GetSection("JwtSettings");
            string secretKey = Environment.GetEnvironmentVariable("secret-key")!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });

            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddTransient<IReportService, ReportServiceRazorLight>();

            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

            services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();
            services.AddScoped<IProductImageStorageService, ProductImageService>();

            services.AddHttpClient();
            services.AddScoped<IOAuthService, FacebookAuthService>();
            services.AddScoped<IOAuthServiceFactory, OAuthServiceFactory>();


            return services;
        }
    }
}
