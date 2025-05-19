using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using OMS_App.Models;
namespace OMS_App.Areas.Post.Models
{
    public class PostBase
    {
        [Key]
        public int PostId { get; set; }

        public string Title { get; set; }
        public string Slug { get; set; }

        public string Content1 { get; set; }

        public bool Published { get; set; }
        public List<PostCategory> PostCategories { get; set; }
    }
    // Model Post
    [Table("Post")]
    public class Post : PostBase
    {
        [Required]

        public string AuthorId { get; set; }


        public AppUser Author { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

    }
    public class PostCategory
    {
        public int PostId { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}