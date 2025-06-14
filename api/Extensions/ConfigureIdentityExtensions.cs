using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;
using infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace api.Extensions
{
    public static class ConfigureIdentityExtensions
    {
        public static void ConfigureIdentityPolicy(
            this IServiceCollection services
        )
        {
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 12;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();
        }
    }
}