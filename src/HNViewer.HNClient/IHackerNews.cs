using System.Collections.Generic;

namespace HNViewer.HNClient
{
    /// <summary>
    /// Main interface for this Hacker News Client Library.
    /// </summary>
    public interface IHackerNews
    {
        /// <summary>
        /// Retrieves the list of new stories.
        /// </summary>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="pageNumber">Index of page to return (one-based).</param>
        /// <param name="titleContains">Only return items whose title contains this substring.</param>
        IEnumerable<HackerNewsItem> GetNewStories(int pageSize, int pageNumber, string titleContains);
    }
}
