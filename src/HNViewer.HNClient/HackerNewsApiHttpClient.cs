using System;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace HNViewer.HNClient
{
    /// <summary>
    /// HttpClient with settings for the Hacker News API
    /// </summary>
    /// <remarks>
    /// This is intended to be used as a singleton.
    /// </remarks>
    public class HackerNewsApiHttpClient : HttpClient
    {
        public HackerNewsApiHttpClient(IOptions<HackerNewsClientOptions> options) 
            : this(new HttpClientHandler(), options)
        {
        }

        public HackerNewsApiHttpClient(HttpMessageHandler handler, IOptions<HackerNewsClientOptions> options)
            : base(handler)
        {
            BaseAddress = new Uri(options.Value.ApiBaseAddress);
        }
    }
}
