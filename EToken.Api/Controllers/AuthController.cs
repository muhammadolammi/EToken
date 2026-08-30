using System.Net.Sockets;
using EToken.Application.Dtos;
using EToken.Application.Interfaces;
using EToken.Domain.Entities;
using EToken.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EToken.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<User> userManager,
    IAccountService accountService,
    JwtService jwtService, RoleManager<IdentityRole<Guid>> roleManager) : ControllerBase
{
        private readonly IAccountService _accountService = accountService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
       
        // 1. Check if CIF or Email already exists
        var existingUser = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName == request.UserName);

        if (existingUser != null)
        {
            return BadRequest(new { message = "User with this Email or UserName already exists." });
        }

        // 2. Instantiate custom User with primary/required data
        var user = new User
        {
            Cif = Guid.NewGuid(),
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName= request.LastName,
            Email= request.Email,
            PhoneNumber= request.PhoneNumber,
            
        };

        // 3. Create user via Identity (handles password hashing and validation automatically)
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        const string defaultRole = "Customer";
    if (!await roleManager.RoleExistsAsync(defaultRole))
    {
        await roleManager.CreateAsync(new IdentityRole<Guid>(defaultRole));
    }
        await userManager.AddToRoleAsync(user, defaultRole);
        var token = await jwtService.GenerateToken(user);

        // auto create an account for the user 
         var account =await _accountService.RegisterAccountAsync(user.Cif,  request.AccountType);


        return Ok(new AuthResponse(token, DateTime.UtcNow.AddHours(2), user.Cif.ToString()));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        
        var user = await userManager.Users
            .FirstOrDefaultAsync(u =>  u.UserName == request.UserName);

        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }


        var token =await jwtService.GenerateToken(user);

        return Ok(new AuthResponse(token, DateTime.UtcNow.AddHours(2), user.Cif.ToString()));
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetUser()
    {

    var cifClaim = User.FindFirst("cif")?.Value;
    if (string.IsNullOrEmpty(cifClaim) || !Guid.TryParse(cifClaim, out var userCif))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }
    
    var user = await userManager.Users.FirstOrDefaultAsync(u =>  u.Id == userCif);
    if (user == null )
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }
    return Ok(new MeResponse(cifClaim, user.FirstName, user.LastName, user.UserName!, user.Email!, user.PhoneNumber! ));
    }

}