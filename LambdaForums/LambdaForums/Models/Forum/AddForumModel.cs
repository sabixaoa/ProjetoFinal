using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LambdaForums.Models.Forum
{
    public class AddForumModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public IFormFile ImageUpload { get; set; }
    }
}
