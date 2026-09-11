using Application.DTOs.Product;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Include(p => p.BranchInventories)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .Include(p => p.BranchInventories)
            .Include(p => p.Reviews)
            .Where(p => p.IsActive)
            .ToListAsync();


    private int LevenshteinDistance(string s, string t)
    {
        int n = s.Length, m = t.Length;
        var d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; i++) d[i, 0] = i;
        for (int j = 0; j <= m; j++) d[0, j] = j;

        for (int i = 1; i <= n; i++)
            for (int j = 1; j <= m; j++)
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + (s[i - 1] == t[j - 1] ? 0 : 1));

        return d[n, m];
    }


    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }



    public async Task<bool> ExistsAsync(int id)
        => await _context.Products.AnyAsync(p => p.Id == id);




    public async Task<ProductDashboardStatsDto> GetProductStatsAsync()
    {
        var activeProducts = _context.Products.Where(p => p.IsActive);

        var avgPrice = await activeProducts.AverageAsync(p =>
            p.DiscountPercentage.HasValue && p.DiscountPercentage > 0
            ? p.Price - (p.Price * p.DiscountPercentage.Value / 100)
            : p.Price);

        var avgRating = await _context.Reviews
            .Where(r => r.Product.IsActive)
            .AverageAsync(r => (double?)r.Rating) ?? 0.0;

        return new ProductDashboardStatsDto
        {
            TotalProducts = await activeProducts.CountAsync(),
            CategoriesCount = await activeProducts.Select(p => p.CategoryId).Distinct().CountAsync(),
            BrandsCount = await activeProducts.Where(p => p.BrandId != null).Select(p => p.BrandId).Distinct().CountAsync(),
            AveragePrice = Math.Round(avgPrice, 2),
            AverageRating = Math.Round(avgRating, 1)
        };
    }


    public async Task<IEnumerable<ChartDataDto>> GetCategoryDistributionAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .GroupBy(p => p.Category.Name)
            .Select(g => new ChartDataDto
            {
                Label = g.Key,
                Value = g.Count()
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ChartDataDto>> GetPriceDistributionAsync()
    {
        var prices = await _context.Products
            .Where(p => p.IsActive)
            .Select(p => p.DiscountPercentage.HasValue
                ? p.Price - (p.Price * p.DiscountPercentage.Value / 100)
                : p.Price)
            .ToListAsync();

        return new List<ChartDataDto>
        {
            new ChartDataDto { Label = "0 - 100 EGP", Value = prices.Count(p => p >= 0 && p < 100) },
            new ChartDataDto { Label = "100 - 200 EGP", Value = prices.Count(p => p >= 100 && p < 200) },
            new ChartDataDto { Label = "200 - 300 EGP", Value = prices.Count(p => p >= 200 && p < 300) },
            new ChartDataDto { Label = "300 - 400 EGP", Value = prices.Count(p => p >= 300 && p < 400) },
            new ChartDataDto { Label = "400 - 500 EGP", Value = prices.Count(p => p >= 400 && p < 500) },
            new ChartDataDto { Label = "500+ EGP", Value = prices.Count(p => p >= 500) }
        };
    }



    public async Task<IEnumerable<Product>> GetFilteredProductsAsync(ProductFilterDto filter)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .AsQueryable();

        if (filter.IsActive.HasValue)
            query = query.Where(p => p.IsActive == filter.IsActive.Value);

        if (filter.CategoryId.HasValue)
        {
            var targetCatId = filter.CategoryId.Value;
            query = query.Where(p => p.CategoryId == targetCatId ||
                                     p.Category.ParentCategoryId == targetCatId);
        }

        if (filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId.Value);

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(p =>
                (p.DiscountPercentage.HasValue && p.DiscountPercentage > 0)
                    ? (p.Price - (p.Price * (p.DiscountPercentage.Value / 100m))) >= filter.MinPrice.Value
                    : p.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p =>
                (p.DiscountPercentage.HasValue && p.DiscountPercentage > 0)
                    ? (p.Price - (p.Price * (p.DiscountPercentage.Value / 100m))) <= filter.MaxPrice.Value
                    : p.Price <= filter.MaxPrice.Value);
        }

        if (filter.HasDiscount.HasValue && filter.HasDiscount.Value)
            query = query.Where(p => p.DiscountPercentage.HasValue && p.DiscountPercentage > 0);

        if (filter.StartDate.HasValue)
            query = query.Where(p => p.CreatedAt >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(p => p.CreatedAt <= filter.EndDate.Value);

        query = filter.SortBy switch
        {
            "PriceAsc" => query.OrderBy(p => p.Price),
            "PriceDesc" => query.OrderByDescending(p => p.Price),
            "Oldest" => query.OrderBy(p => p.CreatedAt),
            "Newest" => query.OrderByDescending(p => p.CreatedAt),
            "TopSales" => query.OrderByDescending(p => p.OrderItems.Count),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var queryBeforeSearch = query;

        List<Product> products;

        if (string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            products = await query.ToListAsync();
        }
        else
        {
            var keywords = filter.SearchTerm
                .Split(new[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);


            var exactQuery = query;
            foreach (var k in keywords)
            {
                var keyword = k; 
                exactQuery = exactQuery.Where(p =>
                    p.Name.Contains(keyword) ||
                    (p.Description != null && p.Description.Contains(keyword)));
            }

            products = await exactQuery.ToListAsync();

            if (products.Count == 0)
            {
                var candidates = await queryBeforeSearch.ToListAsync();

                int GetMaxAllowedDistance(string word)
                {
                    if (word.Length <= 3) return 0;
                    if (word.Length <= 5) return 1;
                    return 2;
                }

                var scored = candidates
                    .Select(p =>
                    {

                        var descriptionPart = string.IsNullOrEmpty(p.Description) ? string.Empty : p.Description;
                        var combinedText = string.IsNullOrEmpty(descriptionPart)
                            ? p.Name
                            : $"{p.Name} {descriptionPart}";

                        var productWords = combinedText
                            .ToLower()
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        int totalScore = 0;
                        bool allMatched = true;

                        foreach (var sw in keywords)
                        {
                            var swLower = sw.ToLower();
                            int maxDistance = GetMaxAllowedDistance(swLower);
                            var bestDistance = productWords
                                .Select(pw => LevenshteinDistance(pw, swLower))
                                .DefaultIfEmpty(int.MaxValue)
                                .Min();

                            if (bestDistance > maxDistance)
                            {
                                allMatched = false;
                                break;
                            }
                            totalScore += bestDistance;
                        }

                        return new { Product = p, AllMatched = allMatched, Score = totalScore };
                    })
                    .Where(x => x.AllMatched)
                    .OrderBy(x => x.Score)
                    .Select(x => x.Product);

                products = filter.SortBy switch
                {
                    "PriceAsc" => scored.OrderBy(p => p.Price).ToList(),
                    "PriceDesc" => scored.OrderByDescending(p => p.Price).ToList(),
                    "Oldest" => scored.OrderBy(p => p.CreatedAt).ToList(),
                    "Newest" => scored.OrderByDescending(p => p.CreatedAt).ToList(),
                    "TopSales" => scored.OrderByDescending(p => p.OrderItems?.Count ?? 0).ToList(),
                    _ => scored.ToList()
                };
            }
        }

        return products;
    }
    public async Task<bool> ToggleProductStatusAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.IsActive = !product.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<List<Product>> GetForPricingAsync(PricingFilterDto filter)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (filter.LowMargin == true)
            query = query.Where(p => ((p.Price - p.PurchasePrice) / p.Price * 100) < 20);

        if (filter.RecentlyUpdated == true)
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            query = query.Where(p => p.UpdatedAt.HasValue && p.UpdatedAt.Value >= sevenDaysAgo);
        }

        var queryBeforeSearch = query;

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var keywords = filter.SearchTerm.Split(new[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            query = query.Where(p => keywords.Any(k => p.Name.Contains(k)));
        }

        var products = await query.ToListAsync();

        // 4. الـ Fuzzy Search
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm) && products.Count == 0)
        {
            var allFilteredProducts = await queryBeforeSearch.ToListAsync();
            var searchWords = filter.SearchTerm.Split(new[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            products = allFilteredProducts
                .Where(p => searchWords.Any(sw =>
                    p.Name.Split(' ').Any(pw => LevenshteinDistance(pw, sw) <= 3)))
                .ToList();

        }

        return products;
    }

    public async Task<PricingStatsDto> GetPricingStatsAsync()
    {
        var activeProducts = _context.Products.Where(p => p.IsActive);

        var totalProducts = await activeProducts.CountAsync();

        var avgMargin = await activeProducts.AnyAsync()
            ? await activeProducts.AverageAsync(p =>
                (p.Price - p.PurchasePrice) / p.Price * 100)
            : 0;

        var todayEgyptStart = DateTimeHelper.EgyptNow.Date.ToUtcFromEgyptTime();
        var priceChangesToday = await activeProducts
            .CountAsync(p => p.UpdatedAt.HasValue && p.UpdatedAt.Value >= todayEgyptStart);

        return new PricingStatsDto
        {
            TotalProducts = totalProducts,
            AvgMarginPercentage = Math.Round(avgMargin, 1),
            PriceChangesToday = priceChangesToday
        };
    }

    public async Task<List<MarginByCategoryDto>> GetMarginByCategoryAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .ToListAsync();

        return products
            .GroupBy(p => p.Category.Name)
            .Select(g => new MarginByCategoryDto
            {
                CategoryName = g.Key,
                AvgMarginPercentage = Math.Round(
                    g.Average(p => (p.Price - p.PurchasePrice) / p.Price * 100), 1)
            })
            .ToList();
    }

    public async Task<List<RevenueByPriceTierDto>> GetRevenueByPriceTierAsync()
    {
        var products = await _context.Products
            .Where(p => p.IsActive)
            .ToListAsync();

        var total = products.Count;
        if (total == 0) return new List<RevenueByPriceTierDto>();

        var premium = products.Count(p => (p.Price - p.PurchasePrice) / p.Price * 100 > 50);
        var standard = products.Count(p =>
        {
            var margin = (p.Price - p.PurchasePrice) / p.Price * 100;
            return margin >= 25 && margin <= 50;
        });
        var budget = products.Count(p => (p.Price - p.PurchasePrice) / p.Price * 100 < 25);

        return new List<RevenueByPriceTierDto>
    {
        new() { Tier = "Premium", Percentage = Math.Round((decimal)premium / total * 100, 1) },
        new() { Tier = "Standard", Percentage = Math.Round((decimal)standard / total * 100, 1) },
        new() { Tier = "Budget", Percentage = Math.Round((decimal)budget / total * 100, 1) }
    };
    }
}