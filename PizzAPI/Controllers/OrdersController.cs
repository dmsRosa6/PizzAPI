using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzAPI.Dtos;
using PizzAPI.Models;

namespace PizzAPI.Controllers
{
    public class OrdersController : Controller
    {
        private readonly PizzaStoreContext _context;

        public OrdersController(PizzaStoreContext pizzaStoreContext)
        {
            _context = pizzaStoreContext;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpGet("{id}/items")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrderItemsById(int id)
        {
            var orderItemsWithPizzas = await _context.OrderItems
                .Where(oi => oi.OrderId == id)
                .Join(
                    _context.Pizzas,
                    oi => oi.PizzaId,
                    p => p.PizzaId,
                    (oi, p) => new
                    {
                        PizzaId = p.PizzaId,
                        PizzaPrice = p.BasePrice,
                        Quantity = oi.Quantity,
                        TotalPrice = p.BasePrice * oi.Quantity
                    })
                .ToListAsync();

            if (orderItemsWithPizzas == null || orderItemsWithPizzas.Count == 0)
            {
                return NotFound();
            }

            return Ok(orderItemsWithPizzas);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateRequest request)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    ClientId = request.ClientId,
                    OrderDate = DateTime.UtcNow
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var dto in request.Items)
                {
                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.OrderId,
                        PizzaId  = dto.PizzaId,
                        Quantity = dto.Quantity
                    });
                }
                await _context.SaveChangesAsync();

                if (request.AddressId.HasValue)
                {
                    _context.Deliveries.Add(new Delivery
                    {
                        OrderId   = order.OrderId,
                        AddressId = request.AddressId.Value,
                        Status    = OrderStatus.InProgress
                    });
                }
                else
                {
                    _context.TakeawayOrders.Add(new TakeawayOrder
                    {
                        OrderId = order.OrderId
                    });
                }
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                // TODO MIGHT RETURN A DTO
                return CreatedAtAction(nameof(GetOrderById), 
                                       new { id = order.OrderId }, 
                                       order);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, $"Could not create order: {ex.Message}");
            }
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<decimal>> GetRevenueBetweenDates([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var revenue = await _context.Orders
                .Where(o => o.OrderDate >= start && o.OrderDate <= end)
                .Join(_context.OrderItems,
                    o => o.OrderId,
                    oi => oi.OrderId,
                    (o, oi) => oi)
                .Join(_context.Pizzas,
                    oi => oi.PizzaId,
                    p => p.PizzaId,
                    (oi, p) => new {
                        Total = p.BasePrice * oi.Quantity
                    })
                .SumAsync(x => x.Total);

            return Ok(revenue);
}

    }
}