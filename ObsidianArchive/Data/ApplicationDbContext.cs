using Microsoft.EntityFrameworkCore;
using ObsidianArchiveWeb.Models;

namespace ObsidianArchiveWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Mystery", DisplayOrder=1 },
                new Category { Id = 2, Name = "Romance", DisplayOrder=2},
                new Category { Id = 3, Name = "Thriller", DisplayOrder=3 }
                );
        }
    }
}
