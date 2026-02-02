using System.Collections.Generic;
using System.Linq;
using ApiHarvester.Models;

namespace ApiHarvester.Services
{
    public class DataProcessor : IDataProcessor
    {
        public SummaryReport GenerateSummary(List<Post> posts, List<User> users, List<Comment> comments, List<Todo> todos)
        {
            // Defensive: null -> empty lists (C# 7.3 compatible)
            if (posts == null) posts = new List<Post>();
            if (users == null) users = new List<User>();
            if (comments == null) comments = new List<Comment>();
            if (todos == null) todos = new List<Todo>();

            // Top users by actual post count (deterministic)
            var topUsers = posts
                .GroupBy(p => p.userId)
                .Select(group => new { UserId = group.Key, PostCount = group.Count() })
                .OrderByDescending(x => x.PostCount)
                .Take(5)
                .Join(users, x => x.UserId, u => u.id,
                      (x, u) => new UserPosts { name = u.name, postCount = x.PostCount })
                .ToList();

            // Most commented posts (deterministic)
            var mostCommentedPosts = comments
                .GroupBy(c => c.postId)
                .Select(group => new { PostId = group.Key, CommentCount = group.Count() })
                .OrderByDescending(x => x.CommentCount)
                .Take(5)
                .Join(posts, x => x.PostId, p => p.id,
                      (x, p) => new PostComments { title = p.title, commentCount = x.CommentCount })
                .ToList();

            // Todos stats
            var completed = todos.Count(t => t.completed);
            var pending = todos.Count - completed;

            return new SummaryReport
            {
                topUsers = topUsers ?? new List<UserPosts>(),
                mostCommentedPosts = mostCommentedPosts ?? new List<PostComments>(),
                todos = new TodoStats { completed = completed, pending = pending }
            };
        }
    }
}
