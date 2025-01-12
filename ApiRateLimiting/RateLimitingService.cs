using Microsoft.Extensions.DependencyInjection;

namespace ApiRateLimiting
{
    public static class RateLimitingServiceExtensions
    {
        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<CustomRateLimitService>(); 
            services.AddScoped<RateLimitFilter>(); 

            services.AddMvcCore(options =>
            {
                options.Filters.Add<RateLimitFilter>();
            });

            return services;
        }
    }
}
