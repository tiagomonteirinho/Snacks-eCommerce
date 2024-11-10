using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Snacks_eCommerce.Context;
using Snacks_eCommerce.Entities;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public OrdersController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet("[action]/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderDetails(int orderId)
    {
        var orderDetails = await _appDbContext.OrderDetails
            .Where(od => od.OrderId == orderId)
            .Include(od => od.Order)
            .Include(od => od.Product)
            .Select(od => new
            {
                Id = od.Id,
                Quantity = od.Quantity,
                Subtotal = od.Total,
                Name = od.Product!.Name,
                ImageUrl = od.Product.ImageUrl,
                Price = od.Product.Price
            })
            .ToListAsync();

        if (orderDetails == null || orderDetails.Count == 0)
        {
            return NotFound("Product details could not be found.");
        }

        return Ok(orderDetails);
    }

    [HttpGet("[action]/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserOrders(int userId)
    {
        var orders = await _appDbContext.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new
            {
                Id = o.Id,
                Total = o.Total,
                OrderDate = o.OrderDate,
            })
            .ToListAsync();

        if (orders == null || orders.Count == 0)
        {
            return NotFound("User orders could not be found.");
        }

        return Ok(orders);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] Order order)
    {
        order.OrderDate = DateTime.Now;
        var shoppingCartItems = await _appDbContext.ShoppingCartItems
            .Where(cart => cart.UserId == order.UserId)
            .ToListAsync();

        if (shoppingCartItems.Count == 0)
        {
            return NotFound("There are no products in the cart.");
        }

        // Begin transaction that groups request actions and only saves changes after all actions have been successfully processed,
        // or else all changes are discarded.
        using (var transaction = await _appDbContext.Database.BeginTransactionAsync())
        {
            try
            {
                _appDbContext.Orders.Add(order);
                await _appDbContext.SaveChangesAsync();

                foreach (var item in shoppingCartItems)
                {
                    var orderDetail = new OrderDetail()
                    {
                        Price = item.UnitPrice,
                        Total = item.Total,
                        Quantity = item.Quantity,
                        ProductId = item.ProductId,
                        OrderId = order.Id,
                    };

                    _appDbContext.OrderDetails.Add(orderDetail);
                }

                await _appDbContext.SaveChangesAsync();
                _appDbContext.ShoppingCartItems.RemoveRange(shoppingCartItems);
                await _appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { OrderId = order.Id });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return BadRequest("Request could not be processed.");
            }
        }
    }
}
