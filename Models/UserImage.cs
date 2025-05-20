using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OMS_App.Models;
[Table("User Image")]
public class UserImage
{
    [Key]
    public int Id { get; set; }
    public string ImagePath { get; set; }

    public string AppUserId { get; set; }
    public bool IsActive { get; set; }

    public AppUser AppUser { get; set; }
}