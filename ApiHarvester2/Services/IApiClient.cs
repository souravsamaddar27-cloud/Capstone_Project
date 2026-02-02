using System.Collections.Generic;
using System.Threading.Tasks;
using ApiHarvester.Models;

namespace ApiHarvester.Services
{
    public interface IApiClient
    {
        Task<List<Post>> FetchPostsAsync();
        Task<List<User>> FetchUsersAsync();
        Task<List<Comment>> FetchCommentsAsync();
        Task<List<Todo>> FetchTodosAsync();
    }
}
