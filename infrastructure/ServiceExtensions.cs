using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.IRepository;
using infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using infrastructure.Data;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;
using infrastructure.Data.Interceptor;

namespace infrastructure
{
    public static class ServiceExtensions
    {
        public static void ConfigureInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment
        )
        {
            //Connection

            var env = environment.EnvironmentName;

            string? connectionString;

            if (env == Environments.Development)
            {
                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                connectionString = configuration.GetConnectionString(
                    isWindows ? "WindowsConnection" : "LinuxConnection"
                );
            }
            else
            {
                connectionString = configuration.GetConnectionString("DefaultConnection");
            }

            services.AddDbContext<ApplicationDbContext>((options) =>
            {
                options.UseSqlServer(connectionString)
                    .AddInterceptors(new SlugInterceptor())
                    .AddInterceptors(new SeriesFormatInterceptor());
            });

            //redis caching
            services.AddStackExchangeRedisCache(options =>
             {
                 options.Configuration = configuration.GetConnectionString("Redis");
                 options.InstanceName = "SampleInstance";
             });

            //Cached Memory
            services.AddMemoryCache();

            //Interface and Repository cached
            //IMemory cached
            services.AddScoped<SeriesRepository>();
            services.AddScoped<ISeriesRepository, CachedSeriesRepository>();

            //Redis
            // services.AddScoped<EpisodeRepository>();
            // services.AddScoped<IEpisodeRepository, CachedEpisodeRepository>();

            //Interface and Repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IEpisodeRepository, EpisodeRepository>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IVideoRepository, VideoRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICommentRepository, CommentRepository>();
        }
    }
}