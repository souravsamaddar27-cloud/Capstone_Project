using System.Collections.Generic;
using ApiHarvester.Models;
using ApiHarvester.Services;

namespace ApiHarvester.Tests
{
    public static class DataProcessorTests
    {
        public static void Run()
        {
            // Arrange
            var posts = new List<Post> {
                new Post { userId = 1, id = 101, title = "A" },
                new Post { userId = 1, id = 102, title = "B" },
                new Post { userId = 2, id = 201, title = "C" }
            };
            var users = new List<User> {
                new User { id = 1, name = "User1" },
                new User { id = 2, name = "User2" }
            };
            var comments = new List<Comment> {
                new Comment { postId = 101, id = 1 },
                new Comment { postId = 101, id = 2 },
                new Comment { postId = 201, id = 3 }
            };
            var todos = new List<Todo> {
                new Todo { userId = 1, completed = true },
                new Todo { userId = 2, completed = false },
                new Todo { userId = 1, completed = true }
            };

            var processor = new DataProcessor();

            // Act
            var summary = processor.GenerateSummary(posts, users, comments, todos);

            // Assert
            Assert.AreEqual("User1", summary.topUsers[0].name, "Top user name mismatch");
            Assert.AreEqual(2, summary.topUsers[0].postCount, "Top user post count mismatch");

            Assert.AreEqual("A", summary.mostCommentedPosts[0].title, "Most commented title mismatch");
            Assert.AreEqual(2, summary.mostCommentedPosts[0].commentCount, "Most commented count mismatch");

            Assert.AreEqual(2, summary.todos.completed, "Todos completed mismatch");
            Assert.AreEqual(1, summary.todos.pending, "Todos pending mismatch");
        }
    }
}
