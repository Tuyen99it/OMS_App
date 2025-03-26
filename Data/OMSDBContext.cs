using Microsoft.EntityFrameworkCore;

using OMS_App.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace OMS_App.Data
{
    public class OMSDBContext : IdentityDbContext<AppUser>
    {

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
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
        }
    }
}