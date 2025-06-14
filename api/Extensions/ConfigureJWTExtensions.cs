using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace api.Extensions
{
    public static class ConfigureJWTExtensions
    {
        public static void ConfigureJWTPolicy(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            //JWT Token Auth
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                options.DefaultScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                var clientId = configuration["Authentication:Google:ClientId"];

                if (clientId == null)
                {
                    throw new ArgumentNullException(nameof(clientId));
                }

                var clientSecret = configuration["Authentication:Google:ClientSecret"];

                if (clientSecret == null)
                {
                    throw new ArgumentNullException(nameof(clientSecret));
                }

                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey("jwt"))
                        {
                            context.Token = context.Request.Cookies["jwt"];
                        }
                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"])
                    )
                };
            });
        }
    }
}