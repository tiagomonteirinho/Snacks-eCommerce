using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Snacks_eCommerce.Context;
using Snacks_eCommerce.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _appDbContext;
    private readonly IConfiguration _config;

    public UsersController(AppDbContext appDbContext, IConfiguration config)
    {
        _appDbContext = appDbContext;
        _config = config;
    }

    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        var checkUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (checkUser != null)
        {
            return BadRequest("That email is already being used.");
        }

        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] User user)
    {
        var currentUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);
        if (currentUser == null)
        {
            return NotFound("Could not sign in.");
        }

        var key = _config["JWT:Key"] ?? throw new ArgumentNullException("JWT:Key", "JWT:Key cannot be null.");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email!)
        };
        var token = new JwtSecurityToken(
            issuer: _config["JWT:Issuer"],
            audience: _config["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(10),
            signingCredentials: credentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return new ObjectResult(new
        {
            AccessToken = jwt,
            TokenType = "bearer",
            UserId = currentUser.Id,
            UserName = currentUser.Name
        });
    }

    [Authorize]
    [HttpPost("uploadimage")]
    public async Task<IActionResult> UploadUserImage(IFormFile image)
    {
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var user = await _appDbContext.Users.FirstOrDefaultAsync(U => U.Email == userEmail);
        if (user == null)
        {
            return NotFound("User could not be found.");
        }

        if (image != null)
        {
            string uniqueFileName = $"{Guid.NewGuid().ToString()}_{image.FileName}";
            string filePath = Path.Combine("wwwroot/userimages", uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            user.ImageUrl = $"/userimages/{uniqueFileName}";
            await _appDbContext.SaveChangesAsync();
            return Ok("Image successfully uploaded.");
        }

        return BadRequest("Image could not be uploaded.");
    }

    [Authorize]
    [HttpGet("userimage")]
    public async Task<IActionResult> GetUserImage()
    {
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null)
        {
            return NotFound("User could not be found.");
        }

        var userImage = await _appDbContext.Users
            .Where(x => x.Email == userEmail)
            .Select(x => new
            {
                x.ImageUrl,
            })
            .SingleOrDefaultAsync();

        return Ok(userImage);
    }
}
