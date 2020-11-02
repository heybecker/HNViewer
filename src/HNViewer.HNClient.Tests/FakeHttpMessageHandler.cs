using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HNViewer.HNClient.Tests
{
    /// <summary>
    /// Helps with mocking API calls.
    /// </summary>
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        public virtual HttpResponseMessage Send(HttpRequestMessage request)
        {
            throw new NotImplementedException("Setup response with mocking framework.");
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }
}
