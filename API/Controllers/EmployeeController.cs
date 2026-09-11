using Application.DTOs.Employee;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[Controller]")]
[Authorize(Policy = "AdminOnly")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
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
            var employee = await _employeeService.GetByIdAsync(id);
            return employee == null
                ? NotFound(new { message = "الموظف مش موجود." })
                : Ok(employee);
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
            var employees = await _employeeService.GetByBranchAsync(branchId);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("role/{role}")]
    public async Task<IActionResult> GetByRole(EmployeeRole role)
    {
        try
        {
            var employees = await _employeeService.GetByRoleAsync(role);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("on-shift")]
    public async Task<IActionResult> GetOnShiftNow()
    {
        try
        {
            var employees = await _employeeService.GetOnShiftNowAsync();
            return Ok(employees);
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

            var employees = await _employeeService.SearchAsync(keyword);
            return Ok(employees);
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
            var stats = await _employeeService.GetStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        try
        {
            var employee = await _employeeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        try
        {
            var result = await _employeeService.UpdateAsync(id, dto);
            return result
                ? Ok(new { message = "تم تحديث بيانات الموظف بنجاح." })
                : NotFound(new { message = "الموظف مش موجود." });
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
            var result = await _employeeService.ToggleStatusAsync(id);
            return result
                ? Ok(new { message = "تم تغيير حالة الموظف بنجاح." })
                : NotFound(new { message = "الموظف مش موجود." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}