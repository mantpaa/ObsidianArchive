using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObsidianArchive.Business.Services.IServices;
using ObsidianArchive.Models;
using ObsidianArchive.Models.ViewModels;
using System.Security.Claims;

namespace ObsidianArchiveWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

        public CartController(IProductService productService, IShoppingCartService shoppingCartService)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItems = await _shoppingCartService.GetCartItemsAsync(userId);

            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = cartItems,
                OrderHeader = new()
            };

            foreach(var cartItem in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;
            }

            return View(shoppingCartVM);
        }

        public async Task<IActionResult> Details(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId, includeCategory: true);

            if (product == null)
            {
                return NotFound();
            }

            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            shoppingCart.ApplicationUserId = userId;

            await _shoppingCartService.AddToCartAsync(shoppingCart);
            return RedirectToAction("Details", new { productId = shoppingCart.ProductId });
        }
    }
}
