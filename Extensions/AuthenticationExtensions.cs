using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SecureAuthDemo.Configuration;
using System.Text;

namespace SecureAuthDemo.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            var jwtSection = config.GetSection("JwtSettings");

            // Adding setting To DI
            services.Configure<JwtSettings>(jwtSection);
            services.Configure<GoogleAuthSettings>(config.GetSection("GoogleAuthSettings"));
            services.Configure<AwsCognitoSettings>(config.GetSection("AwsCognitoSettings"));

            #region JWT Auth
            var jwtSettings = jwtSection.Get<JwtSettings>() 
                ?? throw new InvalidOperationException("JwtSettings missing from configuration.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)),
                    ValidateIssuer = true,
                    ValidateAudience = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true
                };
            });
            #endregion

            // policy/claim
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireClaim("Role", "Admin"));
            });

            return services;
        }
    }
}
