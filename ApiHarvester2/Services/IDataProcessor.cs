using System.Collections.Generic;
using ApiHarvester.Models;

namespace ApiHarvester.Services
{
    public interface IDataProcessor
    {
        SummaryReport GenerateSummary(List<Post> posts, List<User> users, List<Comment> comments, List<Todo> todos);
    }
}
