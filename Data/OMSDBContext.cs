using Microsoft.EntityFrameworkCore;

using OMS_Webapp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace OMS_Webapp.Data
{
    public class OMSDBContext : IdentityDbContext<AppUser>
    {

        public DbSet<AppUser> AppUsers { get; set; }
        public OMSDBContext(DbContextOptions<OMSDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // remove pre-fix AspNet
            foreach(var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var nameTable=entityType.Name;
                if (nameTable.StartsWith("AspNet"))
                {
                    entityType.SetTableName(nameTable.Substring(6));
                }
            }
        }
    }
}