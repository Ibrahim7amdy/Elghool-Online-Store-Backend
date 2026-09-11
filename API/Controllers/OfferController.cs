using Application.DTOs.Offer;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OfferController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OfferController(IOfferService offerService)
    {
        _offerService = offerService;
    }


    // GET: api/Offer
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll([FromQuery] OfferFilter filtre)
    {
        try
        {
            var offers = await _offerService.GetAllOffersAsync(filtre);
            return Ok(offers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/Offer/active
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        try
        {
            var offers = await _offerService.GetActiveOffersAsync();
            return Ok(offers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/Offer/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var offer = await _offerService.GetOfferByIdAsync(id);
            return offer == null
                ? NotFound(new { message = "العرض مش موجود" })
                : Ok(offer);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/Offer
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateOfferDto dto)
    {
        try
        {
            var created = await _offerService.CreateOfferAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/Offer/5
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateOfferDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(new { message = "رقم العرض غير متطابق" });

            var updated = await _offerService.UpdateOfferAsync(dto);
            return updated == false
                ? NotFound(new { message = "العرض مش موجود" })
                : Ok(new { message = "تم تحديث العرض بنجاح" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/Offer/5
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _offerService.DeleteOfferAsync(id);
            return result
                ? Ok(new { message = "تم حذف العرض بنجاح" })
                : NotFound(new { message = "العرض مش موجود" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/Offer/5/products
    [Authorize(Policy = "AdminOnly")]
    [HttpPost("{id}/products")]
    public async Task<IActionResult> AddProduct(int id, [FromBody] AddOfferProductDto dto)
    {
        try
        {
            var result = await _offerService.AddProductToOfferAsync(id, dto);
            return result == false
                ? BadRequest(new { message = "المنتج موجود في العرض بالفعل أو العرض غير موجود" })
                : Ok(new { message = "تم إضافة المنتج للعرض بنجاح" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/Offer/5/products/3
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}/products/{productId}")]
    public async Task<IActionResult> RemoveProduct(int id, int productId)
    {
        try
        {
            var result = await _offerService.RemoveProductFromOfferAsync(id, productId);
            return result == false
                ? NotFound(new { message = "المنتج مش موجود في العرض ده" })
                : Ok(new { message = "تم إزالة المنتج من العرض بنجاح" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PATCH: api/Offer/5/toggle-status
    [Authorize(Policy = "AdminOnly")]
    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var result = await _offerService.ToggleOfferStatusAsync(id);
            return result
                ? Ok(new { message = "تم تغيير حالة العرض بنجاح" })
                : NotFound(new { message = "العرض مش موجود" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/Offer/stats
    [Authorize(Policy = "AdminOnly")]
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = await _offerService.GetOfferStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }



    [Authorize(Policy = "AdminOnly")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportToCsv([FromQuery] OfferFilter filter)
    {
        try
        {
            var offers = await _offerService.GetAllOffersAsync(filter);

            var csv = new StringBuilder();

            csv.AppendLine("العنوان,تاريخ البداية,تاريخ النهاية,نسبة الخصم,سعر الباقة,عدد المنتجات,السعر الإجمالي,عدد الطلبات,الحالة");

            foreach (var item in offers)
            {
                var title = $"\"{item.Title?.Replace("\"", "\"\"")}\"";
                var startDate = item.StartDate.ToString("yyyy-MM-dd");
                var endDate = item.EndDate.ToString("yyyy-MM-dd");
                var discount = item.DiscountPercentage.HasValue ? $"{item.DiscountPercentage}%" : "-";
                var bundlePrice = item.BundlePrice.HasValue ? $"${item.BundlePrice}" : "-";
                var status = $"\"{item.Status}\"";
                csv.AppendLine($"{title},{startDate},{endDate},{discount},{bundlePrice},{item.ProductsCount},${item.TotalOfferPrice},{item.RequestsCount},{status}");
            }

            var bytes = Encoding.UTF8.GetPreamble() // for arabic
                .Concat(Encoding.UTF8.GetBytes(csv.ToString()))
                .ToArray();

            return File(bytes, "text/csv; charset=utf-8", $"Offers_Report_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}