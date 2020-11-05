using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace HNViewer.HNClient.Tests
{
    [TestFixture]
    public class HackerNewsApiHttpClientTests
    {
        private const string FakeAddress = "http://fakeaddress.com/";

        private HackerNewsApiHttpClient _client;

        [SetUp]
        public void SetUp()
        {
            var options = Options.Create(new HackerNewsClientOptions() { ApiBaseAddress = FakeAddress });
            _client = new HackerNewsApiHttpClient(options);
        }

        [Test]
        public void BaseAddress_IsAssigned()
        {
            Assert.AreEqual(FakeAddress, _client.BaseAddress.AbsoluteUri);
        }
    }
}