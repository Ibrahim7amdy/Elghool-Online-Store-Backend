using Application.DTOs.BranchInventory;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AdminOnly")]
public class BranchInventoryController : ControllerBase
{
    private readonly IBranchInventoryService _inventoryService;

    public BranchInventoryController(IBranchInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }


    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        try
        {
            var summary = await _inventoryService.GetStockSummaryAsync();
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "حدث خطأ أثناء جلب إحصائيات المخزون.", error = ex.Message });
        }
    }

    [HttpGet("grid")]
    public async Task<IActionResult> GetGrid([FromQuery] StockFilterDto filter)
    {
        try
        {
            var grid = await _inventoryService.GetStockGridAsync(filter);
            return Ok(grid);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "حدث خطأ أثناء جلب بيانات المخزون.", error = ex.Message });
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] int productId, [FromQuery] int? branchId)
    {
        try
        {
            var history = await _inventoryService.GetStockHistoryAsync(productId, branchId);
            return Ok(history);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "حدث خطأ أثناء جلب سجل الحركات.", error = ex.Message });
        }
    }


    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddInventoryDto dto)
    {
        try
        {
            var result = await _inventoryService.AddInventoryAsync(dto);
            if (!result)
                return BadRequest(new { message = "المنتج أو الفرع مش موجود، أو المخزون موجود بالفعل." });

            return Ok(new { message = "تمت الإضافة وتحديث المخزون بنجاح." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "حدث خطأ أثناء إضافة المخزون.", error = ex.Message });
        }
    }

    [HttpPut("product/{productId}/branch/{branchId}")]
    public async Task<IActionResult> Update(int productId, int branchId, [FromBody] UpdateInventoryDto dto)
    {
        try
        {
            var result = await _inventoryService.UpdateInventoryAsync(productId, branchId, dto);
            if (!result)
                return NotFound(new { message = "المخزون ده مش موجود." });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "حدث خطأ أثناء تعديل المخزون.", error = ex.Message });
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferStock([FromBody] TransferStockDto dto)
    {
        try
        {
            var result = await _inventoryService.TransferStockAsync(dto);
            if (!result)
                return BadRequest(new { message = "فشلت عملية التحويل. تأكد من وجود المخزون الكافي في الفرع المُرسل." });

            return Ok(new { message = "تم تحويل المخزون بنجاح." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "حدث خطأ أثناء عملية التحويل.", error = ex.Message });
        }
    }


    [HttpGet("export")]
    public async Task<IActionResult> ExportToCsv([FromQuery] int? branchId, [FromQuery] string? status)
    {
        var filter = new StockFilterDto
        {
            BranchId = branchId,
            Status = status
        };

        var stockList = await _inventoryService.GetStockGridAsync(filter);

        var csv = new StringBuilder();

        csv.AppendLine("Product Name,Category,Branch,Quantity,Sales Rate,Coverage Days,Status");

        foreach (var item in stockList)
        {
            csv.AppendLine($"\"{item.ProductName}\",\"{item.CategoryName}\",\"{item.BranchName}\",{item.Quantity},\"{item.SalesRate}\",\"{item.CoverageDays} days\",\"{item.Status}\"");
        }

        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv.ToString()))
            .ToArray();

        return File(bytes, "text/csv; charset=utf-8", $"Inventory_Report_{DateTime.UtcNow:yyyyMMdd}.csv");
    }



    [HttpGet("charts/status")]
    public async Task<IActionResult> GetStockStatusChart()
    {
        try
        {
            var chartData = await _inventoryService.GetStockStatusChartAsync();
            return Ok(chartData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "حدث خطأ أثناء جلب البيانات .", error = ex.Message });
        }
    }

    [HttpGet("charts/value")]
    public async Task<IActionResult> GetStockValueChart()
    {
        try
        {
            var chartData = await _inventoryService.GetStockValueChartAsync();
            return Ok(chartData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "حدث خطأ أثناء جلب البيانات.", error = ex.Message });
            }
    }

}