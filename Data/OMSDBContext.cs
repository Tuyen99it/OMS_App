using Microsoft.EntityFrameworkCore;

using OMS_App.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OMS_App.Areas.Post.Models;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Areas.Orders.Models;
namespace OMS_App.Data
{
    public class OMSDBContext : IdentityDbContext<AppUser>
    {

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserImage> UserImages { get; set; }

        // Register Product Category table
        public DbSet<ProductName> ProductNames { get; set; }
        public DbSet<ProductInventory> ProductInventories { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<InventoryImage> InventoryImages { get; set; }
        // Register Orders tables
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderedProduct> OrderedProducts { get; set; }
        public DbSet<OrderAddress> OrderAddresses { get; set; }
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


            // Tạo mối quan hệ many -many between Product and ProductCategory bằng việc tạo key cho bảng bằng việc kết hợp ProductId và CategoryId
            modelBuilder.Entity<CategoryProduct>().HasKey(c => new { c.ProductNameId, c.ProductCategoryId });
            // // Tạo quan hệ giữa appuser và userImage
            modelBuilder.Entity<AppUser>()
                .HasMany(a => a.UserImages)
                .WithOne(u => u.AppUser)
                .HasForeignKey(a => a.AppUserId);

            modelBuilder.Entity<UserImage>()
                .HasOne(u => u.AppUser)
                .WithMany(a => a.UserImages)
                .HasForeignKey(a => a.AppUserId);
            /// Thực hiện trèn 4 sản phẩm vào bảng khi bảng Product được tạo

        }
    }
}