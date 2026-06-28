using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
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

        public OrderStatus Status { get; private set; }

        public int TotalItems { get; private set; }

        public decimal TotalPrice { get; private set; }

        public IList<OrderItemRow> Items { get; private set; } = new List<OrderItemRow>();

        public static IReadOnlyList<OrderStatusStep> StatusSteps { get; } =
        [
            new(OrderStatus.Besteld, "Besteld"),
            new(OrderStatus.InBehandeling, "In behandeling"),
            new(OrderStatus.Verzonden, "Verzonden"),
            new(OrderStatus.Afgeleverd, "Afgeleverd")
        ];

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
            Status = order.Status;
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

        public static string GetStatusLabel(OrderStatus status) => status switch
        {
            OrderStatus.Besteld => "Besteld",
            OrderStatus.InBehandeling => "In behandeling",
            OrderStatus.Verzonden => "Verzonden",
            OrderStatus.Afgeleverd => "Afgeleverd",
            _ => status.ToString()
        };

        public static string GetStatusBadgeClass(OrderStatus status) => status switch
        {
            OrderStatus.Besteld => "bg-secondary",
            OrderStatus.InBehandeling => "bg-warning text-dark",
            OrderStatus.Verzonden => "bg-info text-dark",
            OrderStatus.Afgeleverd => "bg-success",
            _ => "bg-secondary"
        };

        public sealed record OrderItemRow(string Name, int Quantity, decimal UnitPrice)
        {
            public decimal LineTotal => Quantity * UnitPrice;
        }

        public sealed record OrderStatusStep(OrderStatus Status, string Label);
    }
}
