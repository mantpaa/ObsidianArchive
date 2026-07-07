using Microsoft.EntityFrameworkCore;
using ObsidianArchive.Models;

namespace ObsidianArchive.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Mystery", DisplayOrder=1 },
                new Category { Id = 2, Name = "Romance", DisplayOrder=2},
                new Category { Id = 3, Name = "Thriller", DisplayOrder=3 }
                );
            modelBuilder.Entity<Product>().HasData(
                new Product 
                { 
                    Id = 1,
                    Title = "Compendium of the Occult", 
                    Description = "Compendium of the OccultIn seven themed chapters, the book examines the use of talismans and charms, the practice of casting curses, secret societies and sacred sites, divination, rites, and rituals across the world, starting with an introduction to occult practices.",
                    ISBN = "9780500028148", 
                    Author="Liz Williams", 
                    ListPrice = "37.95$", 
                    Price = 30, 
                    Price50 = 25, 
                    Price100 = 20, 
                    ImageUrl = "", 
                    CategoryId=3  
                },
                new Product
                {
                    Id = 2,
                    Title = "another book",
                    Description = "A nother book description",
                    ISBN = "9780500028139",
                    Author = "Made up author",
                    ListPrice = "37.95$",
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 3,
                    Title = "third book",
                    Description = "description here",
                    ISBN = "9780500028200",
                    Author = "Made up author2",
                    ListPrice = "37.95$",
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    ImageUrl = "",
                    CategoryId = 2
                }
                );
        }
    }
}
