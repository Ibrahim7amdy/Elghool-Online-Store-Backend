using Application.DTOs.Dashboard;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardOverviewDto> GetOverviewAsync(DashboardPeriod period);
}