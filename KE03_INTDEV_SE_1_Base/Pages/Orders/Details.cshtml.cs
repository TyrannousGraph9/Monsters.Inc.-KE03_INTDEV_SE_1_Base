using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class OrderDetailsModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;

        public int OrderId { get; private set; }

        public string CustomerName { get; private set; } = string.Empty;

        public string CustomerAddress { get; private set; } = string.Empty;

        public DateTime OrderDate { get; private set; }

        public int TotalItems { get; private set; }

        public decimal TotalPrice { get; private set; }

        public IList<OrderItemRow> Items { get; private set; } = new List<OrderItemRow>();

        public OrderDetailsModel(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IActionResult OnGet(int id)
        {
            var order = _orderRepository.GetOrderById(id);

            if (order is null)
            {
                return NotFound();
            }

            OrderId = order.Id;
            CustomerName = order.Customer.Name;
            CustomerAddress = order.Customer.Address;
            OrderDate = order.OrderDate;
            Items = order.OrderItems
                .Select(item => new OrderItemRow(
                    item.Product.Name,
                    item.Quantity,
                    item.UnitPrice))
                .ToList();

            TotalItems = Items.Sum(item => item.Quantity);
            TotalPrice = Items.Sum(item => item.LineTotal);

            return Page();
        }

        public sealed record OrderItemRow(string Name, int Quantity, decimal UnitPrice)
        {
            public decimal LineTotal => Quantity * UnitPrice;
        }
    }
}