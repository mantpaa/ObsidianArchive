using Microsoft.EntityFrameworkCore;
using ObsidianArchiveWeb.Models;

namespace ObsidianArchiveWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
    }
}
