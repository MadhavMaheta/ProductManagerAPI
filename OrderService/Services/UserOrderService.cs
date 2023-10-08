using OrderService.EFCore;
using OrderService.Models;

namespace OrderService.Services
{
    public class UserOrderService : IUserOrderService, IDisposable
    {
        public ApplicationDbContext _context { get; set; }
        public UserOrderService(ApplicationDbContext context) {
            _context = context;
        }

        public void CreateOrder(Order order)
        {
            _context.Order.Add(order);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
