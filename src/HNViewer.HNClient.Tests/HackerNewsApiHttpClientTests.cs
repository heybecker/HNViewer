using NUnit.Framework;

namespace HNViewer.HNClient.Tests
{
    [TestFixture]
    public class HackerNewsApiHttpClientTests
    {
        private HackerNewsApiHttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _client = new HackerNewsApiHttpClient();
        }

        [Test]
        public void HasBaseAddress()
        {
            Assert.IsNotNull(_client.BaseAddress);
        }
    }
}