using System.Collections.Generic;
using HNViewer.HNClient;
using Microsoft.AspNetCore.Mvc;

namespace HNViewer.Web.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly IHackerNews _hackerNews;

        public HackerNewsController(IHackerNews hackerNews)
        {
            _hackerNews = hackerNews;
        }

        [HttpGet]
        public IEnumerable<HackerNewsItem> NewStories(int pageSize = 30, int pageNumber = 1, string titleContains = null)
        {
            return _hackerNews.GetNewStories(pageSize, pageNumber, titleContains);
        }
    }
}
