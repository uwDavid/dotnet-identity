using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Web_API.Controllers;

[ApiController]
[Route("[controller]")]  // route /auth
public class AuthController : ControllerBase
{
    private readonly IConfiguration configuration;

    public AuthController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpPost]
    public IActionResult Authenticate([FromBody] Credential credential)
    {
        if (credential.UserName == "admin" && credential.Password == "password")
        {
            var claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@website.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", "true"),
                    new Claim("Manager", "true"),
                    new Claim("EmploymentDate", "2024-01-01")
                };

            var expireAt = DateTime.UtcNow.AddMinutes(10);
            return Ok(new
            {
                access_token = CreateToken(claims, expireAt),
                expires_at = expireAt,
            });
            // expire_at appears 2x  
            // 1 - inside jwt 
            // 2 - back to client - for client's convenience
        }

        // incorrect credential
        ModelState.AddModelError("Unathorized", "You are not authorized to access the endpoint.");
        return Unauthorized(ModelState);
    }

    private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
    {
        // get secret key
        // key comes from appsettings.json - 32 k's
        // configuration = IConfiguration DI in constructor
        var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey") ?? "");
        // ?? - if null, use empty string

        // generate JWT
        var jwt = new JwtSecurityToken(
            claims: claims,
            notBefore: DateTime.UtcNow,   //token is not valid before now
            expires: expireAt,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}

public class Credential
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}