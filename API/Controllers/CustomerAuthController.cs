using Application.DTOs.Auth;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/customer/auth")]
public class CustomerAuthController : ControllerBase
{
    private readonly CustomerAuthService _customerAuthService;

    public CustomerAuthController(CustomerAuthService customerAuthService)
    {
        _customerAuthService = customerAuthService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(CustomerRegisterDto dto)
    {
        try
        {
            var response = await _customerAuthService.RegisterAsync(dto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(CustomerLoginDto dto)
    {
        try
        {
            var response = await _customerAuthService.LoginAsync(dto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}