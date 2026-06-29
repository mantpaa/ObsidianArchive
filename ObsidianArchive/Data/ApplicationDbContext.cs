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
                new Category { Id = 1, Name = "Mystery" },
                new Category { Id = 2, Name = "Romance" },
                new Category { Id = 3, Name = "Thriller" }
                );
        }
    }
}
