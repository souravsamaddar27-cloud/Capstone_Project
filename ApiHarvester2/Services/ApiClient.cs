//using System.Net.Http;
//using Newtonsoft.Json;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using ApiHarvester.Models;

//namespace ApiHarvester.Services
//{
//    public class ApiClient : IApiClient
//    {
//        private readonly HttpClient _httpClient;

//        public ApiClient()
//        {
//            _httpClient = new HttpClient();
//        }
//        public ApiClient(HttpClient httpClient)
//        {
//            _httpClient = httpClient ?? new HttpClient();
//        }

//        public async Task<List<Post>> FetchPostsAsync()
//        {
//            return await FetchAsync<List<Post>>("https://jsonplaceholder.typicode.com/posts");
//        }

//        public async Task<List<User>> FetchUsersAsync()
//        {
//            return await FetchAsync<List<User>>("https://jsonplaceholder.typicode.com/users");
//        }

//        public async Task<List<Comment>> FetchCommentsAsync()
//        {
//            return await FetchAsync<List<Comment>>("https://jsonplaceholder.typicode.com/comments");
//        }

//        public async Task<List<Todo>> FetchTodosAsync()
//        {
//            return await FetchAsync<List<Todo>>("https://jsonplaceholder.typicode.com/todos");
//        }

//        private async Task<T> FetchAsync<T>(string url)
//        {
//            var response = await _httpClient.GetAsync(url);
//            response.EnsureSuccessStatusCode();
//            var json = await response.Content.ReadAsStringAsync();
//            return JsonConvert.DeserializeObject<T>(json);
//        }
//    }
//}


using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using ApiHarvester.Models;

namespace ApiHarvester.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10) // prevent hanging calls
            };
        }

        // Optional injectable constructor for testing/mocking
        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        }

        public Task<List<Post>> FetchPostsAsync() => FetchAsync<List<Post>>("https://jsonplaceholder.typicode.com/posts");
        public Task<List<User>> FetchUsersAsync() => FetchAsync<List<User>>("https://jsonplaceholder.typicode.com/users");
        public Task<List<Comment>> FetchCommentsAsync() => FetchAsync<List<Comment>>("https://jsonplaceholder.typicode.com/comments");
        public Task<List<Todo>> FetchTodosAsync() => FetchAsync<List<Todo>>("https://jsonplaceholder.typicode.com/todos");

        private async Task<T> FetchAsync<T>(string url, int maxRetries = 3)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var response = await _httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"HTTP {(int)response.StatusCode} from {url} (attempt {attempt})");
                        if (attempt == maxRetries)
                            throw new HttpRequestException($"Non-success status code: {response.StatusCode}");
                    }
                    else
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(json);
                        if (result == null)
                            throw new JsonException("Deserialized result is null");
                        return result;
                    }
                }
                catch (TaskCanceledException ex) // timeout or cancellation
                {
                    Console.WriteLine($"Timeout on attempt {attempt} for {url}: {ex.Message}");
                    if (attempt == maxRetries) throw;
                }
                catch (HttpRequestException ex) // DNS, connection reset, etc.
                {
                    Console.WriteLine($"Network error on attempt {attempt} for {url}: {ex.Message}");
                    if (attempt == maxRetries) throw;
                }
                catch (JsonException ex) // malformed JSON
                {
                    Console.WriteLine($"JSON parse error for {url}: {ex.Message}");
                    throw; // not retrying bad payloads
                }

                await Task.Delay(TimeSpan.FromSeconds(Math.Min(2 * attempt, 5))); // simple backoff
            }

            throw new Exception($"Retries exhausted for {url}");
        }
    }
}
