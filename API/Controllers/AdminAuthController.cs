using Application.DTOs.Admin;
using Application.DTOs.Auth;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/admin/auth")]
public class AdminAuthController : ControllerBase
{
    private readonly AdminAuthService _adminAuthService;

    public AdminAuthController(AdminAuthService adminAuthService)
    {
        _adminAuthService = adminAuthService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(AdminLoginDto dto)
    {
        try
        {
            var response = await _adminAuthService.LoginAsync(dto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }


    [HttpPost("add")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> AddAdmin([FromBody] CreateAdminDto dto)
    {
        try
        {
            var result = await _adminAuthService.CreateAdminAsync(dto);
            return Ok(new { Message = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}