using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ApiHarvester.Tests
{
    internal class HttpFakeHandler : HttpMessageHandler
    {
        private readonly string _json;
        private readonly HttpStatusCode _status;

        public HttpFakeHandler(string json, HttpStatusCode status = HttpStatusCode.OK)
        {
            _json = json;
            _status = status;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var resp = new HttpResponseMessage(_status)
            {
                Content = new StringContent(_json ?? string.Empty)
            };
            return Task.FromResult(resp);
        }
    }
}
