using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace HNViewer.HNClient
{
    /// <summary>
    /// Default implementation of <see cref="IHackerNewsApi"/>
    /// </summary>
    public class HackerNewsApi : IHackerNewsApi
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly DistributedCacheEntryOptions _responseCacheOptions = new DistributedCacheEntryOptions();

        private readonly HttpClient _httpClient;

        private readonly IDistributedCache _responseCache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="httpClient">Use HackerNewApiHttpClient for correct configuration</param>
        public HackerNewsApi(
            HackerNewsApiHttpClient httpClient, 
            IDistributedCache responseCache, 
            IOptions<HackerNewsClientOptions> options)
        {
            _httpClient = httpClient;
            _responseCache = responseCache;
            _responseCacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.CacheExpirationMinutes);
        }

        /// <summary>
        /// Get list of new story ids.
        /// </summary>
        public Task<IEnumerable<int>> GetNewStoriesAsync()
        {
            return HttpGetAsync<IEnumerable<int>>("newstories.json");
        }

        /// <summary>
        /// Get item details.
        /// </summary>
        public Task<HackerNewsItem> GetItemAsync(int id)
        {
            return HttpGetAsync<HackerNewsItem>($"item/{id}.json");
        }

        private async Task<TResult> HttpGetAsync<TResult>(string url)
        {
            var json = await _responseCache.GetStringAsync(url);

            if (string.IsNullOrEmpty(json))
            {
                json = await _httpClient.GetStringAsync(url);

                await _responseCache.SetStringAsync(url, json, _responseCacheOptions);
            }

            return JsonSerializer.Deserialize<TResult>(json, _jsonSerializerOptions);
        }
    }
}
