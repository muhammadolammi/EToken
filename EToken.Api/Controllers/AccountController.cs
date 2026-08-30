
namespace EToken.Api.Controllers;

using EToken.Application.Dtos;
using EToken.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EToken.Domain.Entities;
using Microsoft.AspNetCore.Identity;

[ApiController]
[Route("api/accounts")]

public class AccountController(IAccountService accountService,     UserManager<User> userManager) : ControllerBase
{ 

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAccount([FromBody]CreateAccountRequest req)
    {
           var cifClaim = User.FindFirst("cif")?.Value;
    if (string.IsNullOrEmpty(cifClaim) || !Guid.TryParse(cifClaim, out var userCif))
    {
        return Unauthorized(new { message = "Invalid token claims." });
    }
        var account =await accountService.RegisterAccountAsync(userCif,  req.AccountType);
        return Ok(account);

    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAccounts( )
    {
           var cifClaim = User.FindFirst("cif")?.Value;
    if (string.IsNullOrEmpty(cifClaim) || !Guid.TryParse(cifClaim, out var userCif))
    {
        return Unauthorized(new { message = "Invalid token claims." });
    }
      var accounts = await accountService.GetAccountsByCifAsync(userCif);

    var response = accounts.Select(a => new AccountResponse(
        Cif: a.Cif.ToString(),
        Id: a.Id.ToString(),
        Number: a.Number,
        Type: a.Type,
        Balance: a.Balance.ToString("F2"), // Formats decimal to "0.00" string
        Status: a.Status.ToString()
    )).ToList();

    return Ok(response);

    }


[HttpGet("name-enquiry")] 
    public async Task<IActionResult> NameEnquiry([FromQuery] string accountNumber )
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
    {
        return BadRequest(new { message = "Account number is required." });
    }

    var cleanNumber = accountNumber.Trim();
    var account = await accountService.GetAccountByNumberAsync(cleanNumber);
        if (account is null)
        
            return NotFound(new { message = "Account number not found." });
        User? user =await  userManager.FindByIdAsync(account.Cif.ToString());
        if (user == null)
        {
        return NotFound(new { message = "User details not found." });

        }
        string accountName = user.FirstName + " " + user.LastName;
        NameEnquiryResponse result = new(account.Cif.ToString(), account.Number, accountName, account.Type);
        return Ok(result);
    }

}