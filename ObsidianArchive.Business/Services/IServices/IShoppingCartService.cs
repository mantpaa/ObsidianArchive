using ObsidianArchive.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObsidianArchive.Business.Services.IServices
{
    public interface IShoppingCartService
    {
        Task<ShoppingCart?> GetCartByIdAsync(int cartId);
        Task<IEnumerable<ShoppingCart>> GetCartItemsAsync(string userId);
        Task<int> GetCartCountAsync(string userId);
        Task<ShoppingCart> AddToCartAsync(ShoppingCart cart);
        Task UpdateCartAsync(ShoppingCart cart);
        Task ClearCartAsync(string userId);
    }
}
