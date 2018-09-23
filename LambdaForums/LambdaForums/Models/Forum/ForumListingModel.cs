using System.ComponentModel.DataAnnotations;

namespace LambdaForums.Models.Forum
{
    public class ForumListingModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Elemento necessário")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Elemento necessário")]
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public int NumberOfPosts { get; set; }
        public int NumberOfUsers { get; set; }
        public bool HasRecentPost { get; set; }
    }
}
