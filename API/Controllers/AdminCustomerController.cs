using Application.DTOs.Customer;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/customers")]
[ApiController]
[Authorize(Policy = "AdminOnly")]
public class AdminCustomerController : ControllerBase
{
    private readonly IAdminCustomerService _customerService;

    public AdminCustomerController(IAdminCustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var customer = await _customerService.GetByIdAsync(id);
            return customer == null
                ? NotFound(new { message = "العميل مش موجود." })
                : Ok(customer);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetByBranch(int branchId)
    {
        try
        {
            var customers = await _customerService.GetByBranchAsync(branchId);
            return Ok(customers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("top-spenders")]
    public async Task<IActionResult> GetTopSpenders()
    {
        try
        {
            var customers = await _customerService.GetTopSpendersAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("new-this-month")]
    public async Task<IActionResult> GetNewThisMonth()
    {
        try
        {
            var customers = await _customerService.GetNewThisMonthAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Keyword is required." });

            var customers = await _customerService.SearchAsync(keyword);
            return Ok(customers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = await _customerService.GetStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var result = await _customerService.ToggleStatusAsync(id);
            return result
                ? Ok(new { message = "تم تغيير حالة العميل بنجاح." })
                : NotFound(new { message = "العميل مش موجود." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}