using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LambdaForums.Data.Models
{
    public class Forum
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public string ImageUrl { get; set; }

        public virtual IEnumerable<Post> Posts { get; set; }
    }
}
