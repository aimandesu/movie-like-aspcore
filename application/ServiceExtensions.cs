using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace application
{
    public static class ServiceExtensions
    {
        public static void ConfigureApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); -> this is for fluent validation
        }
    }
}