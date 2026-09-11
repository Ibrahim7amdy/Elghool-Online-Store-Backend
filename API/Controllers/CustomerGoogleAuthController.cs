using Application.DTOs.Auth;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/customer/auth")]
public class CustomerGoogleAuthController : ControllerBase
{
    private readonly CustomerGoogleAuthService _googleAuthService;

    public CustomerGoogleAuthController(CustomerGoogleAuthService googleAuthService)
    {
        _googleAuthService = googleAuthService;
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin(GoogleAuthDto dto)
    {
        try
        {
            var response = await _googleAuthService.LoginWithGoogleAsync(dto);

            if (response.RequiresAdditionalInfo)
                return Ok(response); // 200 مع RequiresAdditionalInfo = true

            return Ok(response); // 200 مع Token
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}