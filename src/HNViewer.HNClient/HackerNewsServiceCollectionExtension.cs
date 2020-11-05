using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HNViewer.HNClient
{
    public static class HackerNewsServiceCollectionExtension
    {
        public static IServiceCollection AddHackerNewsClient(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<HackerNewsClientOptions>(configuration.GetSection(HackerNewsClientOptions.HackerNewsClient))
                .AddSingleton<HackerNewsApiHttpClient>()
                .AddScoped<IHackerNewsApi, HackerNewsApi>()
                .AddScoped<IHackerNews, HackerNews>();
        }
    }
}
