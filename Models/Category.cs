using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Scheme;
namespace OMS_App
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Phải nhập tên Category")]
        [Display(Name = "Nội dung của category")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        public string Title { get; set; }
        // Nội dung chi tiết về category
        [DataType(DataType.Text)]
        [Display(Name = "Nội dung category")]
        public string Content { get; set; }
        // Chuỗi url của category
        [Required(ErrorMessage = "Phải nhận url")]
        [Display(Name = "Url hiển thị")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ được nhập các ký tự từ a-z , 0-9")]
        public string Slug { get; set; }

        // Category cha ( Foreign key)
        [Display(Name = "ID của danh mục cha")]
        public int? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        [Display(Name = "Danh mục cha")]
        public Category ParentCategory { get; set; }
        // Các category con
        public ICollection<Category> ChildCategories { get; set; }
    }
}