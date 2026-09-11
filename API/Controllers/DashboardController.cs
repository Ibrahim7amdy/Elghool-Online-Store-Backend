using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[Controller]")]
[ApiController]
[Authorize(Policy = "AdminOnly")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // GET: api/dashboard/overview?period=7
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview([FromQuery] int period = 7)
    {
        try
        {
            var dashboardPeriod = period switch
            {
                7 => DashboardPeriod.SevenDays,
                30 => DashboardPeriod.ThirtyDays,
                90 => DashboardPeriod.NinetyDays,
                _ => DashboardPeriod.SevenDays
            };

            var overview = await _dashboardService.GetOverviewAsync(dashboardPeriod);
            return Ok(overview);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}