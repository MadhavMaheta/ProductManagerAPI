using OrderService.Models;

namespace OrderService.Services
{
    public interface IUserOrderService
    {
        public void CreateOrder(Order order);
    }
}
