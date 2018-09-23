using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LambdaForums.Data.Models;
using LambdaForums.Data;
using LambdaForums.Models.ApplicationUser;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace LambdaForums.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;
        private readonly IUpload _uploadService;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IApplicationUser userService,
            IUpload uploadService,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userService = userService;
            _uploadService = uploadService;
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Detail(string id)
        {
            var user = _userService.GetById(id);
            var userRoles = _userManager.GetRolesAsync(user).Result;

            var model = new ProfileModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserRating = user.Rating.ToString(),
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                MemberSince = user.MemberSince,
                IsAdmin = userRoles.Contains("Admin")
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var userId = _userManager.GetUserId(User);

            // Connect to an Azure Storage Account Container
            var connectionString = _configuration.GetConnectionString("AzureStorageAccount");

            // Get Blob Container
            var container = _uploadService.GetBlobContainer(connectionString, "profile-images");

            // Parse the Content Disposition response header
            var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

            // Grab the filename
            var filename = contentDisposition.FileName.Trim('"');

            // Get a reference to a Block Blob
            var blockBlob = container.GetBlockBlobReference(filename);

            // On that block blob, Upload our file <-- file uploaded to the cloud
            await blockBlob.UploadFromStreamAsync(file.OpenReadStream());

            // Set the User's profile image to the URI
            await _userService.SetProfileImage(userId, blockBlob.Uri);

            // Redirect to the user's profile page.
            return RedirectToAction("Detail", "Profile", new { id = userId });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var profiles = _userService.GetAll()
                .OrderByDescending(user => user.Rating)
                .Select(u => new ProfileModel
                {
                    UserId = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    ProfileImageUrl = u.ProfileImageUrl,
                    UserRating = u.Rating.ToString(),
                    MemberSince = u.MemberSince
                });

            var model = new ProfileListModel
            {
                Profiles = profiles
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(ProfileListModel model, string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            //var user = _context.Users.Where(u => u.Id == id);
            if (user != null)
            {
                var replies =  _context.PostReplies.Where(r => r.User.Id == id).ToList();
                replies.ForEach(r => _context.PostReplies.Remove(r));
                var posts = _context.Posts.Where(p => p.User.Id == id).ToList();
                posts.ForEach(p => _context.Posts.Remove(p));
                await _context.SaveChangesAsync();
                //await _userManager.DeleteAsync(user);
                await _userManager.DeleteAsync(user);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Profile");
        }
    }
}