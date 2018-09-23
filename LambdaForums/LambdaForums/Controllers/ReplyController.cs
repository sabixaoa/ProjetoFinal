using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LambdaForums.Data;
using LambdaForums.Data.Models;
using Microsoft.AspNetCore.Identity;
using LambdaForums.Models.Reply;
using Microsoft.AspNetCore.Authorization;

namespace LambdaForums.Controllers
{
    [Authorize]
    public class ReplyController : Controller
    {
        private readonly IPost _postService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;
        private readonly ApplicationDbContext _context;

        public ReplyController(IPost postService, 
            UserManager<ApplicationUser> userManager,
            IApplicationUser userService,ApplicationDbContext context)
        {
            _postService = postService;
            _userManager = userManager;
            _userService = userService;
            _context = context;
        }

        public async Task<IActionResult> Create(int id)
        {
            var post = _postService.GetById(id);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var model = new PostReplyModel
            {
                PostContent = post.Content,
                PostTitle = post.Title,
                PostId = post.Id,

                AuthorId = user.Id,
                AuthorName = User.Identity.Name,
                AuthorImageUrl = user.ProfileImageUrl,
                AuthorRating = user.Rating,
                IsAuthorAdmin = User.IsInRole("Admin"),

                ForumId = post.Forum.Id,
                ForumName = post.Forum.Title,
                ForumImageUrl = post.Forum.ImageUrl,

                Created = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddReply(PostReplyModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            var reply = BuildReply(model, user);

            await _postService.AddReply(reply);
            await _userService.UpdateUserRating(userId, typeof(PostReply));

            return RedirectToAction("Index", "Post", new { id = model.PostId });
        }

        private PostReply BuildReply(PostReplyModel model, ApplicationUser user)
        {
            var post = _postService.GetById(model.PostId);

            return new PostReply
            {
                Post = post,
                Content = model.ReplyContent,
                Created = DateTime.Now,
                User = user
            };
        }
        [HttpGet]
        public async Task<IActionResult> DeleteReply(int id, int post)
        {
            var reply = await _context.PostReplies.FindAsync(id);
            _context.Remove(reply);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Post", new { id = post });
        }
    }
}