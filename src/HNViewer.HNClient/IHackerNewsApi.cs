using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNViewer.HNClient
{
    /// <summary>
    /// Interface for Hacker News API endpoints.
    /// </summary>
    public interface IHackerNewsApi
    {
        /// <summary>
        /// Get list of new story ids.
        /// </summary>
        Task<IEnumerable<int>> GetNewStoriesAsync();

        /// <summary>
        /// Get item details.
        /// </summary>
        Task<HackerNewsItem> GetItemAsync(int id);
    }
}
