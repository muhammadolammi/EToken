namespace EToken.Api.Controllers;

using EToken.Application.Commands;
using EToken.Application.Commons.Interfaces;
using EToken.Application.Dtos;
using EToken.Application.Interfaces;
using EToken.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OtpNet;

[ApiController]
[Route("api/etoken")]
// public class ETokenController(ICustomerDeviceService cDeviceService, IMediator _mediator, ITokenService tokenService) : ControllerBase
public class ETokenController(ICustomerDeviceService cDeviceService, ITokenService tokenService, ISecretStore secretStore) : ControllerBase

{ 
    private readonly ICustomerDeviceService _cDeviceservice = cDeviceService;
    private readonly ITokenService _tokenSerivce = tokenService;

[HttpPost("enrol/init")]
public async Task<IActionResult> InitEnrolment([FromBody]InitEnrolmentRequest req)
    {
    //   make indepontent
    if (req.Cif == Guid.Empty)
    {
              return BadRequest(new { message = "Cif  is Invalid." });

    }
      if (req.DeviceId == Guid.Empty)
    {
              return BadRequest(new { message = "DeviceId  is Invalid." });

    }
    if (req.DeviceModel == "")
    {
              return BadRequest(new { message = "Device Model cant be empty." });

    }



    CustomerDevice? customerDevice = await _cDeviceservice.GetDeviceByIdAsync(req.DeviceId);
    bool isExisting = true;

    if(customerDevice == null)
        {
            isExisting=false;
            // create new device
        customerDevice = await _cDeviceservice.RegisterDeviceAsync(
        cif: req.Cif, deviceId: req.DeviceId,  deviceModel: req.DeviceModel
      );
      if (customerDevice == null)
        {
             return StatusCode(
                StatusCodes.Status500InternalServerError, 
                new { error = "An internal error occurred while processing your enrolment request." }
            ); 
        }
         
       }



  Byte[] encryptedSecret ;

        if (isExisting)
        {
            GetTokenRecord? exitingToken = await _tokenSerivce.GetByDeviceIdAsync( req.DeviceId);
           if (exitingToken == null)
        {
             return StatusCode(
                StatusCodes.Status500InternalServerError, 
                new { error = "An internal error occurred while processing your enrolment request." }
            );
        }
          encryptedSecret= exitingToken.EncryptedSecret;

        }
        else
        {
                //   create a new token 

            TokenProvisionResult tokenProvision = await _tokenSerivce.ProvisionTokenAsync(req.Cif, req.DeviceId);
        if (tokenProvision == null)
        {
             return StatusCode(
                StatusCodes.Status500InternalServerError, 
                new { error = "An internal error occurred while processing your enrolment request." }
            );
        }

        encryptedSecret= tokenProvision.EncryptedSecret;
   
        }
        
       Byte[] rawSecret= await secretStore.Decrypt(encryptedSecret);
       if (rawSecret == null)
        {
             return StatusCode(
                StatusCodes.Status500InternalServerError, 
                new { error = "An internal error occurred while processing your enrolment request." }
            );
        }
         string secretBase32 = Base32Encoding.ToString(rawSecret);
        
        string rsaEncryptedSecret =  RsaDataEncryptor.EncryptForClient(secretBase32, req.DevicePublicKey); 

      InitEnrolmentResponse res = new(req.DeviceId, rsaEncryptedSecret);
     return Ok(res);
    }

  
[HttpPost("enrol/confirm")]
[Authorize(Roles = "Admin")]

    public async Task<IActionResult> ConfirmEnrolment([FromBody] ConfirmEnrolmentRequest req)
{   
  Console.WriteLine($"Incoming DeviceId from JSON: '{req.DeviceId}'");

    if (req.DeviceId == Guid.Empty)
    {
        return BadRequest(new { message = "DeviceId is empty or invalid GUID." });
    }
    var device = await _cDeviceservice.GetDeviceByIdAsync(req.DeviceId);
    if (device is null)
    {
        return NotFound(new { message = "Device not found." });
    }

    await _cDeviceservice.UpdateDeviceStatusAsync(deviceId: req.DeviceId, status: "active");

    return Ok();


    }


// [HttpPost("verify")]
// [Authorize]
// [EnableRateLimiting("etoken-verify")]
// public async Task<IActionResult> Verify(VerifyRequest req)
// {
// // CIF pulled from the authenticated principal, never trusted from the request body
//      var cifClaim = User.FindFirst("cif")?.Value;
//     if (string.IsNullOrEmpty(cifClaim) || !Guid.TryParse(cifClaim, out var userCif))
//     {
//         return Unauthorized(new { message = "Invalid token claims." });
//     }
//     if (req.DeviceId == Guid.Empty)
//     {
//         return BadRequest(new { message = "DeviceId is empty or invalid GUID." });
//     }
    
//     var result = await _mediator.Send(new VerifyCodeCommand(userCif, req.DeviceId, req.Code, req.ActionType));
//     return result.IsValid ? Ok(result) : BadRequest(result);
// }



[HttpPost("revoke")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Revoke(RevokeRequest req)
  {
    await _cDeviceservice.UpdateDeviceStatusAsync(
        deviceId: req.DeviceId, status:"revoked"
      );
     return Ok();
  }




[HttpGet("enrol/status")] 
    public async Task<IActionResult> GetDeviceStatud([FromQuery] string deviceId )
    {
        if (string.IsNullOrWhiteSpace(deviceId))
    {
        return BadRequest(new { message = "Device Id is required." });
    }

    var cleanId = deviceId.Trim();
    Guid deviceGuid = Guid.Parse(cleanId);
    if (deviceGuid == Guid.Empty)
    {
              return BadRequest(new { message = "Device Id is Invalid." });

    }
    var device = await _cDeviceservice.GetDeviceByIdAsync(deviceGuid);
        if (device is null)
        
        return NotFound(new { message = "Device  not found." });
       
        GetDeviceResponse result = new(device.Status);
        return Ok(result);
    }

}