using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<WishList> WishLists => Set<WishList>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<PasswordResetCode> PasswordResetCodes => Set<PasswordResetCode>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<BranchInventory> BranchInventories => Set<BranchInventory>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<OfferProduct> OfferProducts => Set<OfferProduct>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}