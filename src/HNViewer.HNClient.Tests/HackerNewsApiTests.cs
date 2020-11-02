using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace HNViewer.HNClient.Tests
{
    [TestFixture]
    public class HackerNewsApiTests
    {
        FakeHttpMessageHandler _mockHttpMessageHandler;
        IDistributedCache _mockResponseCache;
        IHackerNewsApi _api;

        [SetUp]
        public void SetUp()
        {
            _mockHttpMessageHandler = Substitute.ForPartsOf<FakeHttpMessageHandler>();

            var httpClient = new HttpClient(_mockHttpMessageHandler);
            httpClient.BaseAddress = new Uri("http://fakeaddress.com");

            _mockResponseCache = Substitute.For<IDistributedCache>();

            _api = new HackerNewsApi(httpClient, _mockResponseCache);
        }

        [Test]
        public void GetNewStoriesAsync_CallsNewStoriesEndpoint()
        {
            SetupMockResponse(HttpStatusCode.OK, "[]");

            var result = _api.GetNewStoriesAsync().Result;

            _mockHttpMessageHandler.Received().Send(
                Arg.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("/newstories.json")));
        }

        [Test]
        public void GetNewStoriesAsync_ReturnsIntCollection()
        {
            var jsonString = "[123, 234, 567]";
            var expectedResult = new[] { 123, 234, 567 };

            SetupMockResponse(HttpStatusCode.OK, jsonString);

            var result = _api.GetNewStoriesAsync().Result;

            CollectionAssert.AreEqual(expectedResult, result);
        }

        [Test]
        public void GetNewStoriesAsync_ServerError_ThrowsHttpRequestException()
        {
            SetupMockResponse(HttpStatusCode.InternalServerError, "This is a fake error");

            AssertHttpRequestException(() => { var result = _api.GetNewStoriesAsync().Result; } );
        }

        [Test]
        public void GetItemAsync_CallsItemEndpoint()
        {
            SetupMockResponse(HttpStatusCode.OK, "{ \"id\": 123 }");

            var result = _api.GetItemAsync(123).Result;

            _mockHttpMessageHandler.Received().Send(
                Arg.Is<HttpRequestMessage>(req => req.RequestUri.AbsolutePath.Contains("/item/123.json")));
        }

        [Test]
        public void GetItemAsync_ItemNotInCache_SetsCache()
        {
            const string itemKey = "/item/123.json";

            SetupMockResponse(HttpStatusCode.OK, "{ \"id\": 123 }");

            var result = _api.GetItemAsync(123).Result;

            _mockResponseCache.Received().SetAsync(
                itemKey, 
                Arg.Any<byte[]>(), 
                Arg.Any<DistributedCacheEntryOptions>(), 
                Arg.Any<CancellationToken>());
        }

        [Test]
        public void GetItemAsync_ItemInCache_GetsFromCache()
        {
            const string itemKey = "/item/123.json";
            
            _mockResponseCache
                .GetAsync(itemKey, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Encoding.ASCII.GetBytes("{ \"id\": 123 }")));

            var result = _api.GetItemAsync(123).Result;

            Assert.IsNotNull(result);
            _mockResponseCache.Received().GetAsync(itemKey, Arg.Any<CancellationToken>());
            _mockHttpMessageHandler.DidNotReceive().Send(Arg.Any<HttpRequestMessage>());
        }

        [Test]
        public void GetItemAsync_JsonObjectResponse_ReturnsItem()
        {
            SetupMockResponse(HttpStatusCode.OK, "{ \"id\": 123 }");

            var result = _api.GetItemAsync(123).Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(123, result.Id);
        }

        [Test]
        public void GetItemAsync_JsonNullResponse_ReturnsNull()
        {
            SetupMockResponse(HttpStatusCode.OK, "null");

            var result = _api.GetItemAsync(-1).Result;

            Assert.IsNull(result);
        }

        [Test]
        public void GetItemAsync_ServerError_ThrowsHttpRequestException()
        {
            SetupMockResponse(HttpStatusCode.InternalServerError, "This is a fake error");

            AssertHttpRequestException(() => { var result = _api.GetItemAsync(123).Result; });
        }

        private void SetupMockResponse(HttpStatusCode statusCode, string content)
        {
            _mockHttpMessageHandler.Send(Arg.Any<HttpRequestMessage>()).Returns(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
        }

        private void AssertHttpRequestException(TestDelegate code)
        {
            var aggEx = Assert.Throws<AggregateException>(code);
            Assert.That(aggEx.GetBaseException(), Is.TypeOf<HttpRequestException>());
        }
    }
}