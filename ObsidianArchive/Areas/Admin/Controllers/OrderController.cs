using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ObsidianArchive.Business.Services.IServices;
using ObsidianArchive.DataAccess.Data;
using ObsidianArchive.Models;
using ObsidianArchive.Models.ViewModels;
using ObsidianArchive.Utility;
using System.Security.Claims;

namespace ObsidianArchiveWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.RoleAdmin)]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICategoryService _categoryService;

        [BindProperty]
        public OrderHeader OrderHeader { get; set; }
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            
            return View();
        }



        [AllowAnonymous]
        public async Task<IActionResult> Details(int orderId)
        {
            OrderHeader = await _orderService.GetOrderByIdAsync(orderId, includeUser:true, includeDetails: true);
            return View(OrderHeader);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
        public async Task<IActionResult> UpdateOrderDetails()
        {
            var orderHeaderFromDb = await _orderService.GetOrderByIdAsync(OrderHeader.Id);
            orderHeaderFromDb.Name = OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderHeader.City;
            orderHeaderFromDb.State = OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderHeader.Carrier) && orderHeaderFromDb.OrderStatus == OrderStatus.Shipped.ToString())
            {
                orderHeaderFromDb.Carrier = OrderHeader.Carrier;
            }

            if (!string.IsNullOrEmpty(OrderHeader.TrackingNumber) && orderHeaderFromDb.OrderStatus == OrderStatus.Shipped.ToString())
            {
                orderHeaderFromDb.TrackingNumber = OrderHeader.TrackingNumber;
            }

            await _orderService.UpdateOrderAsync(orderHeaderFromDb);

            TempData["success"] = "Order details updated successfully.";
            return RedirectToAction(nameof(Details), orderHeaderFromDb.Id);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
        public async Task<IActionResult> UpdateOrderStatus(string status)
        {
            var orderHeader = await _orderService.GetOrderByIdAsync(OrderHeader.Id);
            if (orderHeader == null)
            {
                TempData["error"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }

            string successMessage;

            switch(status)
            {
                case nameof(OrderStatus.Processing):
                    await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, OrderStatus.Processing.ToString(), OrderHeader.Carrier, OrderHeader.TrackingNumber);
                    successMessage = "Order processing started successfully.";
                    break;
                case nameof(OrderStatus.Cancelled):
                    await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, OrderStatus.Cancelled.ToString(), OrderHeader.Carrier, OrderHeader.TrackingNumber);
                    successMessage = "Order cancelled successfully.";
                    break;
                case nameof(OrderStatus.Refunded):
                    await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, OrderStatus.Refunded.ToString(), OrderHeader.Carrier, OrderHeader.TrackingNumber);
                    successMessage = "Order refunded successfully.";
                    break;
                case nameof(OrderStatus.Shipped):

                    if (string.IsNullOrEmpty(OrderHeader.Carrier) || string.IsNullOrEmpty(OrderHeader.TrackingNumber))
                    {
                        TempData["error"] = "Carrier and Tracking Number are required to ship the order.";
                        return RedirectToAction(nameof(Details), new {orderId = OrderHeader.Id});
                    }
                    await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, OrderStatus.Shipped.ToString(), OrderHeader.Carrier, OrderHeader.TrackingNumber);
                    successMessage = "Order shipped successfully.";
                    break;
                default:
                    TempData["error"] = "Invalid status update.";
                    return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
            }

            TempData["success"] = successMessage;
            return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
        }

        #region API calls
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(string status)
        {
            string? userId = null; // will only remain null if user is admin or employee.

            // if user is not admin or employee, we get their userId. 
            if (!User.IsInRole(StaticDetails.RoleAdmin) && !User.IsInRole(StaticDetails.RoleEmployee))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
            }
            if (status == "all") status = ""; // empty status will get all results.
            var orders = await _orderService.GetAllOrdersAsync(userId, status);
            return Json(new { data = orders });
        }

       
        #endregion
    }
}
