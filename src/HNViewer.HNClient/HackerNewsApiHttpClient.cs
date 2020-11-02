using System;
using System.Net.Http;

namespace HNViewer.HNClient
{
    /// <summary>
    /// HttpClient with settings for the Hacker News API
    /// </summary>
    public class HackerNewsApiHttpClient : HttpClient
    {
        public HackerNewsApiHttpClient()
            : base()
        {
            //TODO: settings file
            BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
        }
    }
}
