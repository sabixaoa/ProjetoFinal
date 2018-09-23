using LambdaForums.Data;
using LambdaForums.Data.Models;
using LambdaForums.Service;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace LambdaForums.Tests
{
    [TestFixture]
    public class Post_Service_Should 
    {
        [TestCase("coffee", 3)]
        [TestCase("teA", 1)]
        [TestCase("water", 0)]
        public void Return_Filtered_Results_Corresponding_To_Query(string query, int expected)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            //Arrange
            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.Forums.Add(new Forum
                {
                    Id = 19
                });

                ctx.Posts.Add(new Post
                {
                    Forum = ctx.Forums.Find(19),
                    Id = 23523,
                    Title = "First Post",
                    Content = "Coffee"
                });

                ctx.Posts.Add(new Post
                {
                    Forum = ctx.Forums.Find(19),
                    Id = -2144,
                    Title = "Coffee",
                    Content = "Some Content"
                });

                ctx.Posts.Add(new Post
                {
                    Forum = ctx.Forums.Find(19),
                    Id = 223,
                    Title = "Tea",
                    Content = "Coffee"
                });

                ctx.SaveChanges();
            }

            // Act
            using (var ctx = new ApplicationDbContext(options))
            {
                var postService = new PostService(ctx);
                var result = postService.GetFilteredPosts(query);
                var postCount = result.Count();

                // Assert
                Assert.AreEqual(expected, postCount);
            }
        }
    }
}
