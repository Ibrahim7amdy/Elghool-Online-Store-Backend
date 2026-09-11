using Application.DTOs.Brand;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class BrandController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(brands);
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
            var brand = await _brandService.GetBrandByIdAsync(id);
            return brand == null
                ? NotFound(new { message = "البراند ده مش موجود" })
                : Ok(brand);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
    {
        try
        {
            var brand = await _brandService.CreateBrandAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(new { message = "رقم البراند مش متوافق" });

            var updated = await _brandService.UpdateBrandAsync(dto);
            return updated == false
                ? NotFound(new { message = "البراند ده مش موجود" })
                : Ok(new { message = "تم تحديث البراند بنجاح" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _brandService.DeleteBrandAsync(id);
            return result
                ? Ok(new { message = "تم حذف البراند بنجاح" })
                : NotFound(new { message = "البراند ده مش موجود" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}