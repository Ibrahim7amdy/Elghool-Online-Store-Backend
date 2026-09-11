using Application.Interfaces.Services;
using Application.Services;
using Application.Validators.Auth;
using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationDependencies
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // ── Services ─────────────────────────────────────────────────
        services.AddScoped<CustomerAuthService>();
        services.AddScoped<AdminAuthService>();
        services.AddScoped<PasswordResetService>();
        services.AddScoped<CustomerGoogleAuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<BranchService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IOfferService, OfferService>();
        services.AddScoped<ICustomerProfileService, CustomerProfileService>();
        services.AddScoped<IBranchInventoryService, BranchInventoryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IWishListService, WishListService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IAdminCustomerService, AdminCustomerService>();
        services.AddScoped<IDashboardService, DashboardService>();

        // ── FluentValidation ─────────────────────────────────────────
        services.AddValidatorsFromAssemblyContaining<CustomerRegisterValidator>();

        return services;
    }
}