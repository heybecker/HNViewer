using System;
using System.Collections.Generic;
using System.Text;

namespace HNViewer.HNClient
{
    public class HackerNewsClientOptions
    {
        public const string HackerNewsClient = "HackerNewsClient";

        public string ApiBaseAddress { get; set; }

        public int CacheExpirationMinutes { get; set; }
    }
}
