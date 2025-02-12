using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.MongoDb;
using Backend.MongoDb.Model;
using Backend.Service.Authentication;
using Backend.Service.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController :ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IUserRepository userRepository;

    public AuthController(IConfiguration configuration,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IUserRepository userRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        this.userRepository = userRepository;
        _configuration = configuration;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new ApplicationUser { UserName = model.Username, Email = model.Email};
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        var userData = new UserData
        {
            Name = model.Username,
            GamesPlayed = 0,
            GamesWin =new GamesWin(),
            SimulationStateIds = new List<string>()
        };
        
        await userRepository.CreateAsync(userData);
        
        user.UserDataId = userData.Id;
        await _userManager.UpdateAsync(user);


        return Ok(new { message = "User registered successfully!" });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null || !(await _userManager.CheckPasswordAsync(user, model.Password)))
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = GenerateJwtToken(user);
        
        Response.Cookies.Append("access_token", token, new CookieOptions
        {
            HttpOnly = true, //against xss
            Secure = true, //https is needed
            SameSite = SameSiteMode.Strict, //against CSRF
            Expires = DateTime.UtcNow.AddMinutes(30)
        } );
    
        return Ok(new {message = "access guaranted" });
      
    }
    
    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, "User")
        };
        
        var secret = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            expires: DateTime.UtcNow.AddHours(1),
            claims:claims,
            signingCredentials: creds
        );


        return new JwtSecurityTokenHandler().WriteToken(token);
        
    }

}