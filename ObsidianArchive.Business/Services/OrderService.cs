using Microsoft.EntityFrameworkCore;
using ObsidianArchive.Business.Services.IServices;
using ObsidianArchive.DataAccess.Data;
using ObsidianArchive.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObsidianArchive.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<OrderHeader> CreateOrderAsync(OrderHeader orderHeader)
        {
            _dbContext.OrderHeaders.Add(orderHeader);
            await _dbContext.SaveChangesAsync();
            return orderHeader;
        }

        public async Task<IEnumerable<OrderHeader>> GetAllOrdersAsync(string? userId = null, string? status = null, bool includeUser = false, bool includeDetails = false)
        {
            var query = _dbContext.OrderHeaders.AsQueryable();
            if (includeUser)
            {
                query = query.Include(o => o.ApplicationUser);
            }

            if (includeDetails)
            {
                query = query.Include(o => o.OrderDetails).ThenInclude(od => od.Product);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(u => u.ApplicationUserId == userId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(u => u.OrderStatus == status);
            }

            return await query.ToListAsync();
        }

        public async Task<OrderHeader?> GetOrderByIdAsync(int id, bool includeUser = false, bool includeDetails = false)
        {
            var query = _dbContext.OrderHeaders.AsQueryable();
            if (includeUser)
            {
                query = query.Include(o => o.ApplicationUser);
            }

            if (includeDetails)
            {
                query = query.Include(o => o.OrderDetails).ThenInclude(od => od.Product);
            }
            
            return await query.FirstOrDefaultAsync(o=>o.Id == id);
        }

        public Task<OrderHeader?> UpdateOrderAsync(OrderHeader orderHeader)
        {
            throw new NotImplementedException();
        }
    }
}
