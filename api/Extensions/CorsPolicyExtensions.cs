using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Extensions
{
    public static class CorsPolicyExtensions
    {
        public static void ConfigureCorsPolicy(this IServiceCollection services)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            services.AddCors(options =>
            {
                options.AddPolicy(name:
                    MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials()
                                .WithOrigins("http://localhost:5173");
                    }
                );
            });
        }
    }
}