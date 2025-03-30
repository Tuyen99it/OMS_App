using Microsoft.EntityFrameworkCore;

using OMS_App.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OMS_App.Areas.Post.Models;
namespace OMS_App.Data
{
    public class OMSDBContext : IdentityDbContext<AppUser>
    {

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts {get;set;}
        public DbSet<PostCategory> PostCategories {get;set;}
        public OMSDBContext(DbContextOptions<OMSDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // remove pre-fix AspNet
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var nameTable = entityType.Name;
                if (nameTable.StartsWith("AspNet"))
                {
                    entityType.SetTableName(nameTable.Substring(6));
                }
            }

            // Đánh chỉ mục cho Categor 
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(p => p.Slug);
            });
            // Tạo mối quan hệ many -many between Post and Category bằng việc tạo key cho bảng bằng việc kết hợp PostId và CategoryId
            modelBuilder.Entity<PostCategory>().HasKey(p=>new {p.PostId, p.CategoryId});
        }
    }
}