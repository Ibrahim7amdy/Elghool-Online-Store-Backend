using Application.DTOs.Customer;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/customer/profile")]
[Authorize(Policy = "CustomerOnly")]
public class CustomerProfileController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ICustomerProfileService _customerProfileService;

    public CustomerProfileController(
        ICustomerRepository customerRepository,
        IBranchRepository branchRepository,
        ICustomerProfileService customerProfileService)
    {
        _customerRepository = customerRepository;
        _branchRepository = branchRepository;
        _customerProfileService = customerProfileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var customerId = int.Parse(User.Claims.First(c =>
            c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            var profile = await _customerProfileService.GetProfileAsync(customerId);
            return Ok(profile);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateCustomerProfileDto dto)
    {
        var customerId = int.Parse(User.Claims.First(c =>
            c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            await _customerProfileService.UpdateProfileAsync(customerId, dto);
            return Ok(new { message = "تم تحديث البيانات بنجاح" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var customerId = int.Parse(User.Claims.First(c =>
            c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            await _customerProfileService.ChangePasswordAsync(customerId, dto);
            return Ok(new { message = "تم تغيير الباسورد بنجاح" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}