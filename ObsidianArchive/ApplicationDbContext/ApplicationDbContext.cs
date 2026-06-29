using Microsoft.EntityFrameworkCore;
using ObsidianArchiveWeb.Models;

namespace ObsidianArchiveWeb.ApplicationDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
    }
}
