using ObsidianArchive.Business.Services.IServices;
using ObsidianArchive.DataAccess.Data;
using ObsidianArchive.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObsidianArchive.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }
           
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
            return category; // maybe fetch the last category to ensure it is correct.
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var foundCat = await GetCategoryByIdAsync(id);
            if (foundCat == null)
            {
                throw new KeyNotFoundException($"Category with {id} not found.");
            }

            _context.Remove(foundCat);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
       

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsCategoryNameUniqueAsync(string name, int? categoryId = null)
        {
           if (categoryId.HasValue && categoryId.Value != 0)
            {
                return !await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != categoryId.Value);
            }
            else
            {
                return !await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower());
            }
        }
    }
}
