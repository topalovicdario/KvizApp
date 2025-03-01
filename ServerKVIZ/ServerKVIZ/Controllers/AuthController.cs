using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServerKVIZ.Repositoryes;
using ServerKVIZ.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("[controller]")]
public partial class AuthController : ControllerBase
{
    private readonly IAuthentificatable playerAuth;
    private readonly IConfiguration configurationOfToken;

    public AuthController(IAuthentificatable playerAuth, IConfiguration config)
    {
        this.playerAuth = playerAuth;
        this.configurationOfToken = config;
    }

    [HttpPost("authMe")]
    public async Task<IActionResult> AuthenticateUser([FromBody] AuthRequest request)
    {
        if (await playerAuth.Authentificate(request.Nickname, request.Password))
        {
            var token = GenerateJwtToken(request.Nickname);
            return Ok(new { Token = token });
        }
        return Unauthorized();
    }

 
    private string GenerateJwtToken(string nickname)
    {
        var jwtSettings = configurationOfToken.GetSection("Jwt");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.Name, nickname),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30), // Preporučuje se kraće trajanje tokena
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

