using Application.DTOs.Product;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/Product
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/Product/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product == null
                ? NotFound(new { message = "المنتج مش موجود" })
                : Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    // POST: api/Product
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/Product/5
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto)
    {
        try
        {
            var updated = await _productService.UpdateProductAsync(id, dto);
            return updated == false
                ? NotFound(new { message = "المنتج مش موجود" })
                : Ok(new { message = "تم تحديث المنتج بنجاح" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("dashboard/stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var stats = await _productService.GetProductStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("dashboard/charts/category-distribution")]
    public async Task<IActionResult> GetCategoryDistributionChart()
    {
        try
        {
            var chartData = await _productService.GetCategoryDistributionAsync();
            return Ok(chartData);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("dashboard/charts/price-distribution")]
    public async Task<IActionResult> GetPriceDistributionChart()
    {
        try
        {
            var chartData = await _productService.GetPriceDistributionAsync();
            return Ok(chartData);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("App/filter")]
    public async Task<IActionResult> AppGetFilteredProducts([FromQuery] ProductFilterDto filter)
    {
        filter.IsActive = true;
        try
        {
            var result = await _productService.AppGetFilteredProductsAsync(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("dashboard/filter")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DashboardGetFilteredProducts([FromQuery] ProductFilterDto filter)
    {
        try
        {
            var result = await _productService.DashboardGetFilteredProducts(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPatch("dashboard/{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var result = await _productService.ToggleProductStatusAsync(id);
            return result == false
                ? NotFound(new { message = "المنتج مش موجود" })
                : Ok(new { message = "تم تغيير حالة المنتج بنجاح" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [Authorize(Policy = "AdminOnly")]
    [HttpGet("pricing/table")]
    public async Task<IActionResult> GetPricingTable([FromQuery] PricingFilterDto filter)
    {
        try
        {
            var result = await _productService.GetPricingTableAsync(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("pricing/stats")]
    public async Task<IActionResult> GetPricingStats()
    {
        try
        {
            var stats = await _productService.GetPricingStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("pricing/charts/margin-by-category")]
    public async Task<IActionResult> GetMarginByCategoryChart()
    {
        try
        {
            var chartData = await _productService.GetMarginByCategoryChartAsync();
            return Ok(chartData);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("pricing/charts/revenue-by-price-tier")]
    public async Task<IActionResult> GetRevenueByPriceTierChart()
    {
        try
        {
            var chartData = await _productService.GetRevenueByPriceTierChartAsync();
            return Ok(chartData);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/price")]
    public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdateSinglePriceDto dto)
    {
        var result = await _productService.UpdateProductPriceAsync(id, dto);

        if (!result)
            return NotFound(new { Message = "المنتج غير موجود" });

        return Ok(new { Message = "تم تحديث السعر بنجاح" });
    }
}