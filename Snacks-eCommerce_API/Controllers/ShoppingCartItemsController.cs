using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Snacks_eCommerce.Context;
using Snacks_eCommerce.Entities;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class ShoppingCartItemsController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public ShoppingCartItemsController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    //[HttpGet("{userId}")]
    //public async Task<IActionResult> Get(int userId)
    //{
    //    var user = await _appDbContext.Users.FindAsync(userId);
    //    if (user is null)
    //    {
    //        return NotFound("User could not be found.");
    //    }

    //    var shoppingCartItems = await (from s in _appDbContext.ShoppingCartItems.Where(s => s.UserId == userId)
    //                                   join p in _appDbContext.Products on s.ProductId equals p.Id
    //                                   select new
    //                                   {
    //                                       Id = s.Id,
    //                                       Price = s.Price,
    //                                       Total = s.Total,
    //                                       Quantity = s.Quantity,
    //                                       ProductId = p.Id,
    //                                       ProductName = p.Name,
    //                                       ImageUrl = p.ImageUrl
    //                                   }).ToListAsync();
    //    return Ok(shoppingCartItems);
    //}
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetShoppingCartItems(int userId)
    {
        var user = await _appDbContext.Users.FindAsync(userId);
        if (user is null)
        {
            return NotFound("User could not be found.");
        }

        var shoppingCartItems = await _appDbContext.ShoppingCartItems
            .Where(s => s.UserId == userId)
            .Include(s => s.Product)
            .Select(s => new
            {
                Id = s.Id,
                Quantity = s.Quantity,
                Price = s.UnitPrice,
                Total = s.Total,
                ProductId = s.Product!.Id,
                ProductName = s.Product.Name,
                ImageUrl = s.Product.ImageUrl
            })
            .ToListAsync();

        if (shoppingCartItems == null || shoppingCartItems.Count == 0)
        {
            return NotFound("Shopping cart items could not be found.");
        }

        return Ok(shoppingCartItems);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ShoppingCartItem shoppingCartItem)
    {
        try
        {
            var shoppingCart = await _appDbContext.ShoppingCartItems.FirstOrDefaultAsync(s =>
                s.ProductId == shoppingCartItem.ProductId && s.UserId == shoppingCartItem.UserId);

            if (shoppingCart != null)
            {
                shoppingCart.Quantity += shoppingCartItem.Quantity;
                shoppingCart.Total = shoppingCart.UnitPrice * shoppingCart.Quantity;
            }
            else
            {
                var product = await _appDbContext.Products.FindAsync(shoppingCartItem.ProductId);
                var cart = new ShoppingCartItem()
                {
                    UserId = shoppingCartItem.UserId,
                    ProductId = shoppingCartItem.ProductId,
                    UnitPrice = shoppingCartItem.UnitPrice,
                    Quantity = shoppingCartItem.Quantity,
                    Total = (product!.Price) * (shoppingCartItem.Quantity)
                };

                _appDbContext.ShoppingCartItems.Add(cart);
            }

            await _appDbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Request could not be processed.");
        }
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(int productId, string action)
    {
        // Get user email through token.
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user is null)
        {
            return NotFound("User could not be found.");
        }

        var shoppingCartItem = await _appDbContext.ShoppingCartItems.FirstOrDefaultAsync(s =>
            s.UserId == user!.Id && s.ProductId == productId);

        if (shoppingCartItem != null)
        {
            if (action.ToLower() == "increase")
            {
                shoppingCartItem.Quantity += 1;
            }
            else if (action.ToLower() == "decrease")
            {
                if (shoppingCartItem.Quantity > 1)
                {
                    shoppingCartItem.Quantity -= 1;
                }
                else
                {
                    _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                    await _appDbContext.SaveChangesAsync();
                    return Ok();
                }
            }
            else if (action.ToLower() == "remove")
            {
                _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                await _appDbContext.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("Request could not be processed. Available action field values: 'increase'; 'decrease'; 'remove'.");
            }

            shoppingCartItem.Total = shoppingCartItem.UnitPrice * shoppingCartItem.Quantity;
            await _appDbContext.SaveChangesAsync();
            return Ok($"Request successfully processed.");
        }
        else
        {
            return NotFound("There are no products in the cart.");
        }
    }
}
