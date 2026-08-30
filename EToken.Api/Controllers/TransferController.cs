namespace EToken.Api.Controllers;

using EToken.Application.Dtos;
using EToken.Application.Interfaces;
using EToken.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/transfers")]
[Authorize]
public class TransferController(ITransferService transferService) : ControllerBase
{
   

    [HttpPost]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest req)
    {
        var cifClaim = User.FindFirst("cif")?.Value;
        if (string.IsNullOrEmpty(cifClaim) || !Guid.TryParse(cifClaim, out var userCif))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }

        try
        {
            var response = await transferService.ProcessTransferAsync(userCif, req);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message }); // E-Token invalid
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message }); // Insufficient funds / invalid amount
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}