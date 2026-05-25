using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class OrdersModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;

        public IList<OrderSummary> Orders { get; private set; } = new List<OrderSummary>();

        public OrdersModel(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public void OnGet()
        {
            Orders = _orderRepository
                .GetAllOrders()
                .OrderByDescending(order => order.OrderDate)
                .ThenByDescending(order => order.Id)
                .Select(order => new OrderSummary(
                    order.Id,
                    order.Customer.Name,
                    order.Customer.Address,
                    order.OrderItems.Sum(item => item.Quantity),
                    order.OrderItems.Select(item => item.Product.Name).ToList(),
                    order.OrderDate))
                .ToList();
        }

        public sealed record OrderSummary(int Id, string CustomerName, string CustomerAddress, int TotalItems, IList<string> Products, DateTime OrderDate)
        {
            public int OrderNumber => Id;

            public string ProductSummary => string.Join(", ", Products);
        }
    }
}