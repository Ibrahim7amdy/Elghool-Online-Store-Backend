using Application.Interfaces.Repositories;
using Application.Interfaces.Repository;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Infrastructure.Persistance.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ── Database Connection ──────────────────────────────────────
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Database connection string 'DefaultConnection' is missing.");
        }

        // ── Repositories ─────────────────────────────────────────────
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IPasswordResetCodeRepository, PasswordResetCodeRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IBranchInventoryRepository, BranchInventoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IWishListRepository, WishListRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();

        // ── Infrastructure & Security Services ───────────────────────
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IFileService, AzureBlobStorageService>();
        //builder.Services.AddScoped<IFileService, FileService>(); // for local


        return services;
    }
}