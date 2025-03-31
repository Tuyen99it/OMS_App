using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using OMS_App.Models;
namespace OMS_App.Areas.Post.Models
{
    public class PostBase
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Please enter the post title")]
        [Display(Name ="Tiêu đề")]
        [StringLength(160,MinimumLength =3,ErrorMessage ="{0} dài từ {1} đến {2}")]
        public string Title { get; set; }
        [Display(Name = "Mô tả ngắn")]
        public string Description { get; set; }
        [Display(Name ="Chuỗi định danh (url)",Prompt ="Nhập hoặc để trống sẽ tự phát sinh theo tiêu đề")]
        [Required(ErrorMessage ="Phải thiết lập địa chỉ url")]
        [StringLength(160,MinimumLength =3,ErrorMessage ="{0} dài từ {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        public string Slug {get;set;}
        [Display(Name ="Nội dung")]
        public string Content1 {get;set;}
        [Display(Name = "Xuất bản")]
        public bool Published {get;set;}
        public List<PostCategory>PostCategories {get;set;}
    }
    // Model Post
    [Table("Post")]
    public class Post: PostBase{
        [Required]
        [Display(Name ="Tác giả")]
        public string AuthorId {get;set;}
        [ForeignKey("AuthorId")]
        [Display(Name ="Tác giả")]

        public AppUser Author {get;set;}
        [Display(Name ="Ngày tạo")]
        public DateTime DateCreated{get;set;}
        [Display(Name ="Ngày cập nhật")]
        public DateTime DateUpdated {get;set;}

    }
    public class PostCategory {
        public int PostId {get;set;}
        public int CategoryId {get;set;}
        [ForeignKey("PostId")]
        public Post Post{get;set;}
        [ForeignKey("CategoryId")]
        public Category Category {get;set;}
    }
}