using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using ApiHarvester.Models;
using ApiHarvester.Services;

namespace ApiHarvester.Tests
{
    // Fake handler to return canned JSON without real network calls
    internal class FakeHandler : HttpMessageHandler
    {
        private readonly string _json;
        public FakeHandler(string json) { _json = json; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_json)
            };
            return Task.FromResult(response);
        }
    }

    public static class ApiClientTests
    {
        public static async Task RunAsync()
        {
            // Arrange: sample JSON for two posts
            var sample = JsonConvert.SerializeObject(new List<Post> {
                new Post { userId = 1, id = 1, title = "Hello" },
                new Post { userId = 2, id = 2, title = "World" }
            });

            var httpClient = new HttpClient(new FakeHandler(sample));
            var api = new ApiClient(httpClient);

            // Act
            var posts = await api.FetchPostsAsync();

            // Assert
            Assert.AreEqual(2, posts.Count, "Posts count mismatch");
            Assert.AreEqual("Hello", posts[0].title, "First post title mismatch");
            Assert.AreEqual(1, posts[0].userId, "First post userId mismatch");
        }
    }
}
