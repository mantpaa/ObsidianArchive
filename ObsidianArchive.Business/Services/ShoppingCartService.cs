using Microsoft.EntityFrameworkCore;
using ObsidianArchive.Business.Services.IServices;
using ObsidianArchive.DataAccess.Data;
using ObsidianArchive.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObsidianArchive.Business.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartService(ApplicationDbContext db)
        {
            _context = db;
        }

        public async Task<ShoppingCart?> GetCartByIdAsync(int cartId)
        {
            return await _context.ShoppingCarts.Include(u => u.Product).FirstOrDefaultAsync(u => u.Id == cartId);
        }

        public async Task<IEnumerable<ShoppingCart>> GetCartItemsAsync(string userId)
        {
            return await _context.ShoppingCarts.Include(u => u.Product).Where(u => u.ApplicationUserId == userId).ToListAsync();
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            return await _context.ShoppingCarts.Where(u => u.ApplicationUserId == userId).SumAsync(u => u.Count);
        }

        public async Task ClearCartAsync(string userId)
        {
            var cartItems =  await _context.ShoppingCarts.Include(u => u.Product).Where(u => u.ApplicationUserId == userId).ToListAsync();
            if (cartItems.Any())
            {
                _context.ShoppingCarts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ShoppingCart> AddToCartAsync(ShoppingCart cart)
        {
            var foundCart = await _context.ShoppingCarts.Include(u=> u.Product).FirstOrDefaultAsync(u => u.ApplicationUserId == cart.ApplicationUserId && u.ProductId == cart.ProductId);
            if (foundCart != null)
            {
                foundCart.Count += cart.Count;
                await _context.SaveChangesAsync();
                return foundCart;
            }
            else
            {
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
                return cart;
            }
        }

        public async Task UpdateCartAsync(ShoppingCart cart)
        {
            if (cart.Count <= 0)
            {
                _context.ShoppingCarts.Remove(cart);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.ShoppingCarts.Update(cart);
                await _context.SaveChangesAsync();
            }
        }
    }
}
