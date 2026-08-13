using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObsidianArchive.Business.Services.IServices;
using ObsidianArchive.Models;
using ObsidianArchive.Models.ViewModels;
using ObsidianArchive.Utility;
using System.Security.Claims;

namespace ObsidianArchiveWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IProductService _productService;

        public CartController(IOrderService orderService, IShoppingCartService shoppingCartService, IApplicationUserService applicationUserService, IProductService productService)
        {
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _applicationUserService = applicationUserService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItems = await _shoppingCartService.GetCartItemsAsync(userId);
            var user = await _applicationUserService.GetUserByIdAsync(userId);

            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = cartItems,
                OrderHeader = new()
            };

            shoppingCartVM.OrderHeader.ApplicationUser = user;
            shoppingCartVM.OrderHeader.ApplicationUserId = user.Id;
            shoppingCartVM.OrderHeader.Name = user.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            shoppingCartVM.OrderHeader.State = user.State;
            shoppingCartVM.OrderHeader.City = user.City;
            shoppingCartVM.OrderHeader.PostalCode = user.PostalCode;
            shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;

            foreach (var cartItem in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;
            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPOST(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItems = await _shoppingCartService.GetCartItemsAsync(userId);
            var user = await _applicationUserService.GetUserByIdAsync(userId);

            shoppingCartVM.ShoppingCartList = cartItems;

            shoppingCartVM.OrderHeader.OrderDate = DateTime.UtcNow;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;
            

            foreach (var cartItem in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;
            }

            shoppingCartVM.OrderHeader.OrderStatus = OrderStatus.Approved.ToString();
            shoppingCartVM.OrderHeader.OrderDetails = shoppingCartVM.ShoppingCartList.Select(cart => new OrderDetails
            {
                ProductId = cart.ProductId,
                Price = cart.Price,
                Count = cart.Count
            }).ToList();

            await _orderService.CreateOrderAsync(shoppingCartVM.OrderHeader);
            return RedirectToAction("OrderConfirmation", new {id = shoppingCartVM.OrderHeader.Id});
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            return View(id);
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

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                if (cart.Count >= 1000)
                {
                    cart.Count = 1000;
                }
                else
                {
                    cart.Count++;
                }

                await _shoppingCartService.UpdateCartAsync(cart);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                cart.Count--;
                await _shoppingCartService.UpdateCartAsync(cart);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                cart.Count = 0;
                await _shoppingCartService.UpdateCartAsync(cart);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateCart(int cartId, int count)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                return NotFound();
            }

            if (count <= 1)
            {
                cart.Count = 0;
            }
            else
            {
                if (count >= 1000)
                {
                    cart.Count = 1000;
                }
                else
                {
                    cart.Count = count;
                }
            }

            await _shoppingCartService.UpdateCartAsync(cart);
            return Ok(new { success = true });
        }
    }
}
