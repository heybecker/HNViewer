using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace HNViewer.HNClient.Tests
{
    [TestFixture]
    public class HackerNewsTests
    {
        private IEnumerable<int> _itemIds = Enumerable.Range(1, 500).ToList();
        private IHackerNewsApi _mockApi;
        private IHackerNews _hackerNews;

        [SetUp]
        public void SetUp()
        {
            _mockApi = Substitute.For<IHackerNewsApi>();
            _mockApi.GetNewStoriesAsync().Returns(_itemIds);
            _mockApi.GetItemAsync(Arg.Any<int>()).Returns(
                callInfo =>
                {
                    int id = callInfo.Arg<int>();
                    return Task.FromResult(new HackerNewsItem() { Id = id, Title = $"item number {id}" });
                });

            _hackerNews = new HackerNews(_mockApi);
        }

        [Test]
        public void GetNewStories_ReturnsItemCollectionInOrder()
        {
            var items = _hackerNews.GetNewStories(100, 1, null);
            
            Assert.That(items, Is.Not.Null.And.Not.Empty);
            CollectionAssert.AreEqual(
                Enumerable.Range(1, 100).ToList(), 
                items.Select(item => item.Id).ToList());
        }

        [Test]
        [TestCase(1, 1, 1, Description = "first page, one item")]
        [TestCase(100, 1, 100, Description = "first page, one hundred items")]
        [TestCase(100, 5, 100, Description = "last page, one hundred items")]
        [TestCase(200, 3, 100, Description = "last page, two hundred items, page half full")]
        [TestCase(1000, 1, 500, Description = "all items one page")]
        public void GetNewStories_PageSize_ReturnsCorrectCount(int pageSize, int pageNumber, int expectedCount)
        {
            Assert.AreEqual(expectedCount, _hackerNews.GetNewStories(pageSize, pageNumber, null).Count());
        }

        [Test]
        public void GetNewStories_PageSizeLessThan1_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _hackerNews.GetNewStories(-1, 1, null), "Page size must be a positive number");
        }

        [Test]
        [TestCase(1, 1, 1, 1, Description = "first item")]
        [TestCase(10, 1, 1, 10, Description = "items 1-10")]
        [TestCase(10, 3, 21, 30, Description = "items 21-30")]
        [TestCase(10, 50, 491, 500, Description = "items 491-500")]
        [TestCase(150, 4, 451, 500, Description = "items 451-500")]
        public void GetNewStories_PageNumber_ReturnsCorrectItems(int pageSize, int pageNumber, int expectedFirstId, int expectedLastId)
        {
            var items = _hackerNews.GetNewStories(pageSize, pageNumber, null);

            Assert.AreEqual(expectedFirstId, items.First().Id);
            Assert.AreEqual(expectedLastId, items.Last().Id);
        }

        [Test]
        public void GetNewStories_PageNumberLessThan1_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _hackerNews.GetNewStories(1, -1, null), "Page number must be a positive number");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void GetNewStories_TitleFilterNullOrEmpty_DoesNotFilter(string titleFilter)
        {
            var items = _hackerNews.GetNewStories(1000, 1, titleFilter);

            Assert.AreEqual(500, items.Count());
        }

        [Test]
        public void GetNewStories_TitleFilterWithMatches_ReturnsCorrectItems()
        {
            var titleFilter = "11";

            var items = _hackerNews.GetNewStories(1000, 1, titleFilter);

            CollectionAssert.AreEqual(
                _itemIds.Where(id => id.ToString().Contains(titleFilter)).ToList(),
                items.Select(item => item.Id).ToList());
        }

        [Test]
        public void GetNewStories_TitleFilterWithoutMatches_ReturnsEmptyCollection()
        {
            var items = _hackerNews.GetNewStories(1000, 1, "no such title");

            Assert.That(items, Is.Not.Null.And.Empty);
        }

        [Test]
        [TestCase("number")]
        [TestCase("NUMBER")]
        [TestCase("NuMbEr")]
        public void GetNewStories_TitleFilter_IsCaseInsensitive(string titleFilter)
        {
            var items = _hackerNews.GetNewStories(10, 1, titleFilter);

            Assert.AreEqual(10, items.Count());
        }
    }
}