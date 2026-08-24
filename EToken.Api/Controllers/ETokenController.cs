namespace EToken.Api.Controllers;

using EToken.Api.Dtos;
using EToken.Application.Interfaces;
using EToken.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using EToken.Application.Commands;
[ApiController]
[Route("api/etoken")]
public class ETokenController(ICustomerDeviceService cDeviceService) : ControllerBase
{ 
    private readonly ICustomerDeviceService _cDeviceservice = cDeviceService;

[HttpPost("enrol/init")]
[Authorize] // requires an already-authenticated session
public async Task<IActionResult> InitEnrolment(InitEnrolmentRequest req)
    {
      CustomerDevice res = await _cDeviceservice.RegisterDeviceAsync(
        cif: req.cif, deviceModel: req.device_model
      );
     return Created();
    }

  
[HttpPost("enrol/confirm")]
[Authorize]
public async Task<IActionResult> ConfirmEnrolment(ConfirmEnrolmentRequest req)
    {   
        await _cDeviceservice.UpdateDeviceStatusAsync(
        deviceId: req.device_id, status:"active"
      );
     return Ok();

    }


[HttpPost("verify")]
[Authorize]
[EnableRateLimiting("etoken-verify")]
public async Task<IActionResult> Verify(VerifyRequest req)
{
// CIF pulled from the authenticated principal, never trusted from the request body
var cif = User.FindFirst("cif")!.Value;
var result = await _mediator.Send(new VerifyCodeCommand(cif, req.device_id, req.code, req.action_type));
return result.IsValid ? Ok(result) : BadRequest(result);
}



[HttpPost("revoke")]
[Authorize]
public async Task<IActionResult> Revoke(RevokeRequest req)
  {
    await _cDeviceservice.UpdateDeviceStatusAsync(
        deviceId: req.device_id, status:"revoked"
      );
     return Ok();
  }
}