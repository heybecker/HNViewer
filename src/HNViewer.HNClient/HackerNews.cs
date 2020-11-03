using System;
using System.Collections.Generic;
using System.Linq;

namespace HNViewer.HNClient
{
    /// <summary>
    /// Default implementation of <see cref="IHackerNews"/>
    /// </summary>
    public class HackerNews : IHackerNews
    {
        private readonly IHackerNewsApi _api;

        public HackerNews(IHackerNewsApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Retrieves the list of new stories.
        /// </summary>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="pageNumber">Index of page to return (one-based).</param>
        /// <param name="titleContains">Only return items whose title contains this substring.</param>
        public IEnumerable<HackerNewsItem> GetNewStories(int pageSize, int pageNumber, string titleContains)
        {
            if (pageSize < 1) throw new ArgumentException("Page size must be a positive number");
            if (pageNumber < 1) throw new ArgumentException("Page number must be a positive number");

            return _api.GetNewStoriesAsync().Result
                .AsParallel()
                .AsOrdered()
                .Select(id => _api.GetItemAsync(id).Result)
                .Where(item =>
                    string.IsNullOrEmpty(titleContains) ||
                    (item.Title != null && item.Title.Contains(titleContains, StringComparison.InvariantCultureIgnoreCase)))
                .Skip(pageSize * (pageNumber-1))
                .Take(pageSize)
                .ToList();
        }
    }
}
