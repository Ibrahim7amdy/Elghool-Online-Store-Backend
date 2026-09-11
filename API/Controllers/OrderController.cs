using Application.DTOs.Order;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // ===== Customer Endpoints =====

    // POST: api/order
    [Authorize(Policy = "CustomerOnly")]
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var order = await _orderService.CreateOrderAsync(customerId, dto);
            return CreatedAtAction(nameof(GetMyOrderById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/order/my
    [Authorize(Policy = "CustomerOnly")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var orders = await _orderService.GetMyOrdersAsync(customerId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/order/my/5
    [Authorize(Policy = "CustomerOnly")]
    [HttpGet("my/{id}")]
    public async Task<IActionResult> GetMyOrderById(int id)
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id, customerId);
            if (order == null) return NotFound("الأوردر مش موجود.");
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/order/my/5/cancel
    [Authorize(Policy = "CustomerOnly")]
    [HttpDelete("my/{id}/cancel")]
    public async Task<IActionResult> CancelMyOrder(int id)
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _orderService.CancelOrderAsync(id, customerId);
            if (!result) return NotFound("الأوردر مش موجود.");
            return Ok(new { message = "تم إلغاء وحذف الأوردر بنجاح." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ===== Admin Endpoints =====

    // GET: api/order
    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/order/branch/5
    [Authorize(Policy = "AdminOnly")]
    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetOrdersByBranch(int branchId)
    {
        try
        {
            var orders = await _orderService.GetOrdersByBranchAsync(branchId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/order/5
    [Authorize(Policy = "AdminOnly")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAdminAsync(id);
            if (order == null) return NotFound("الأوردر مش موجود.");
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/order/5/status
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto);
            if (!result) return NotFound("الأوردر مش موجود.");
            return Ok(new { message = "تم تحديث حالة الأوردر بنجاح." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/order/5/cancel
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        try
        {
            var result = await _orderService.CancelOrderByAdminAsync(id);
            if (!result) return NotFound("الأوردر مش موجود.");
            return Ok(new { message = "تم إلغاء وحذف الأوردر بنجاح." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}