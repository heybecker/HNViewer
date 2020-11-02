using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="httpClient">Use HackerNewApiHttpClient for correct configuration</param>
        public HackerNewsApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
            return await _httpClient
                .GetStringAsync(url)
                .ContinueWith(t => JsonSerializer.Deserialize<TResult>(t.Result, _jsonSerializerOptions));
        }
    }
}
