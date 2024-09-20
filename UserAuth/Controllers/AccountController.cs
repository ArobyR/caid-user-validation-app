using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserAuth.Models;
using UserAuth.Services;

namespace UserAuth.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RevokedTokenService _revokedTokenService;

    public AccountController(RevokedTokenService revokedTokenService)
    {
        _revokedTokenService = revokedTokenService;
    }
    public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String),
                    new Claim(ClaimTypes.Name, user.Name, ClaimValueTypes.String)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            HttpContext.Session.SetString("Name", user.Name);

            return Ok(new { token = tokenHandler.WriteToken(token), user.Name });
        }
        return Unauthorized();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutViewModel model)
    {
        if (string.IsNullOrEmpty(model.Token))
        {
            return BadRequest("Token is required.");
        }

        if (_revokedTokenService.IsTokenRevoked(model.Token))
        {
            return BadRequest("Token has already been revoked.");
        }

        await _revokedTokenService.RevokeTokenAsync(model.Token);

        return Ok(new { message = "Logout successful, token revoked." });
    }

}
