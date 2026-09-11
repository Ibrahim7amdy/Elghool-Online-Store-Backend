using Application.DTOs.Branch;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class BranchController : ControllerBase
{
    private readonly BranchService _branchService;

    public BranchController(BranchService branchService)
    {
        _branchService = branchService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var branches = await _branchService.GetAllAsync();
            return Ok(branches);
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
            var branch = await _branchService.GetByIdAsync(id);
            return branch == null
                ? NotFound(new { message = "الفرع مش موجود" })  
                : Ok(branch);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create(BranchInputDto dto)
    {
        try
        {
            var branch = await _branchService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = branch.Id }, branch);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BranchInputDto dto)
    {
        try
        {
            var branch = await _branchService.UpdateAsync(id, dto);
            return branch == null
                ? NotFound(new { message = "الفرع مش موجود" })  
                : Ok(branch);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var result = await _branchService.ToggleStatusAsync(id);
            return result
                ? Ok(new { message = "تم تغيير حالة الفرع بنجاح." })
                : NotFound(new { message = "الفرع مش موجود." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("stats")]
    public async Task<IActionResult> GetAllStats([FromQuery] BranchFilterDto? filter)
    {
        try
        {
            var stats = await _branchService.GetFilteredBranchesStatsAsync(filter ?? new BranchFilterDto());
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }



    [Authorize(Policy = "AdminOnly")]
    [HttpGet("{id}/stats")]
    public async Task<IActionResult> GetStats(int id)
    {
        try
        {
            var stats = await _branchService.GetBranchStatsAsync(id);
            return stats == null
                ? NotFound(new { message = "الفرع مش موجود" }) 
                : Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("top-performers")]
    public async Task<IActionResult> GetTopPerformers()
    {
        try
        {
            var stats = await _branchService.GetTopPerformersAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("needs-attention")]
    public async Task<IActionResult> GetNeedsAttention()
    {
        try
        {
            var stats = await _branchService.GetNeedsAttentionAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}