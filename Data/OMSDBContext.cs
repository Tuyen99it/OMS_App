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
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<Product> Products { get; set; }
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
            modelBuilder.Entity<PostCategory>().HasKey(p => new { p.PostId, p.CategoryId });

            /// Thực hiện trèn 4 sản phẩm vào bảng khi bảng Product được tạo
            modelBuilder.Entity<Product>().HasData(
               new Product()
               {
                   ProductId = 1,
                   Name = "Đá phong thuỷ tự nhiên",
                   Description = "Số 1 cao 40cm rộng 20cm dày 20cm màu xanh lá cây đậm",
                   Price = 1000000
               },
                new Product()
                {
                    ProductId = 2,
                    Name = "Đèn đá muối hình tròn",
                    Description = "Trang trí trong nhà Chất liệu : • Đá muối",
                    Price = 1500000
                },
                new Product()
                {
                    ProductId = 3,
                    Name = "Tranh sơn mài",
                    Description = "Tranh sơn mài loại nhỏ 15x 15 giá 50.000",
                    Price = 50000
                },
                new Product()
                {
                    ProductId = 4,
                    Name = "Tranh sơn dầu - Ngựa",
                    Description = "Nguyên liệu thể hiện :    Sơn dầu",
                    Price = 450000
                }
            );
        }
    }
}