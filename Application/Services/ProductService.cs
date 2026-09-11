using Application.DTOs.Product;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Helpers;


namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IFileService _fileService;

    public ProductService(IProductRepository productRepository, IFileService fileService)
    {
        _productRepository = productRepository;
        _fileService = fileService;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return null;
        return MapToDto(product);
    }

    //public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword)
    //{
    //    var products = await _productRepository.SearchAsync(keyword);

    //    if (!products.Any())
    //        products = await _productRepository.FuzzySearchAsync(keyword);

    //    return products.Select(MapToDto).ToList();
    //}

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            DiscountPercentage = dto.DiscountPercentage,
            UnitType = dto.UnitType,
            Weight = dto.Weight,
            WeightUnit = dto.WeightUnit,
            CategoryId = dto.CategoryId,
            PurchasePrice = dto.PurchasePrice,
            BrandId = dto.BrandId,
            IsActive = true
        };

        if (dto.Images != null && dto.Images.Any())
        {
            int order = 1;
            foreach (var image in dto.Images)
            {
                var imageUrl = await _fileService.UploadFileAsync(image, "products");
                product.Images.Add(new ProductImage
                {
                    ImageUrl = imageUrl,
                    DisplayOrder = order++
                });
            }
        }

        await _productRepository.AddAsync(product);
        var createdProduct = await _productRepository.GetByIdAsync(product.Id);
        return MapToDto(createdProduct!);
    }

    public async Task<bool> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return false;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.DiscountPercentage = dto.DiscountPercentage;
        product.UnitType = dto.UnitType;
        product.PurchasePrice = dto.PurchasePrice;
        product.Weight = dto.Weight;
        product.WeightUnit = dto.WeightUnit;
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;

        if (dto.ImageIdsToDelete != null && dto.ImageIdsToDelete.Any())
        {
            var imagesToDelete = product.Images
                .Where(i => dto.ImageIdsToDelete.Contains(i.Id))
                .ToList();

            foreach (var image in imagesToDelete)
            {
                await _fileService.DeleteFileAsync(image.ImageUrl);
                product.Images.Remove(image);
            }
        }

        if (dto.Images != null && dto.Images.Any())
        {
            int order = product.Images.Any() ? product.Images.Max(i => i.DisplayOrder) + 1 : 1;
            foreach (var image in dto.Images)
            {
                var imageUrl = await _fileService.UploadFileAsync(image, "products");
                product.Images.Add(new ProductImage
                {
                    ImageUrl = imageUrl,
                    DisplayOrder = order++
                });
            }
        }

        await _productRepository.UpdateAsync(product);
        return true;
    }



    public async Task<ProductDashboardStatsDto> GetProductStatsAsync()
    {
        return await _productRepository.GetProductStatsAsync();
    }


    public async Task<IEnumerable<ChartDataDto>> GetCategoryDistributionAsync()
    {
        return await _productRepository.GetCategoryDistributionAsync();
    }

    public async Task<IEnumerable<ChartDataDto>> GetPriceDistributionAsync()
    {
        return await _productRepository.GetPriceDistributionAsync();
    }


    public async Task<IEnumerable<ProductDto>> AppGetFilteredProductsAsync(ProductFilterDto filter)
    {
        var products = await _productRepository.GetFilteredProductsAsync(filter);

        return products.Select(MapToDto).ToList();
    }



    public async Task<List<DashboardProductDto>> DashboardGetFilteredProducts(ProductFilterDto filter)
    {
        var products = await _productRepository.GetFilteredProductsAsync(filter);

        return products.Select(p => new DashboardProductDto
        {
            Id = p.Id,
            Name = p.Name,
            CategoryName = p.Category?.Name,
            BrandName = p.Brand?.Name,
            OldPrice = p.Price,
            NewPrice = p.DiscountPercentage.HasValue
                ? p.Price - (p.Price * p.DiscountPercentage.Value / 100)
                : p.Price,
            DiscountPercentage = p.DiscountPercentage,
            AverageRating = (double)(p.Reviews?.Any() == true ? Math.Round(p.Reviews.Average(r => r.Rating), 1) : 0),
            ReviewsCount = p.Reviews?.Count ?? 0,
            IsActive = p.IsActive
        }).ToList();
    }



    public async Task<bool> ToggleProductStatusAsync(int id)
    {
        return await _productRepository.ToggleProductStatusAsync(id);
    }



    public async Task<List<PricingItemDto>> GetPricingTableAsync(PricingFilterDto filter)
    {
        var products = await _productRepository.GetForPricingAsync(filter);

        return products.Select(p => new PricingItemDto
        {
            ProductId = p.Id,
            ItemName = p.Name,
            CategoryName = p.Category?.Name,
            PurchasePrice = p.PurchasePrice,
            SellingPrice = p.Price,
            MarginPercentage = Math.Round((p.Price - p.PurchasePrice) / p.Price * 100, 1),
            Brand = p.Brand?.Name,
            LastUpdated = p.UpdatedAt.ToEgyptTime()
        }).ToList();
    }

    public async Task<PricingStatsDto> GetPricingStatsAsync()
        => await _productRepository.GetPricingStatsAsync();

    public async Task<List<MarginByCategoryDto>> GetMarginByCategoryChartAsync()
        => await _productRepository.GetMarginByCategoryAsync();

    public async Task<List<RevenueByPriceTierDto>> GetRevenueByPriceTierChartAsync()
        => await _productRepository.GetRevenueByPriceTierAsync();



    public async Task<bool> UpdateProductPriceAsync(int productId, UpdateSinglePriceDto dto)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return false;

        if (dto.PurchasePrice.HasValue)
            product.PurchasePrice = dto.PurchasePrice.Value;

        if (dto.SellingPrice.HasValue)
            product.Price = dto.SellingPrice.Value;

        product.UpdatedAt = DateTimeHelper.EgyptNow.ToUtcFromEgyptTime();

        await _productRepository.UpdateAsync(product);
        return true;
    }



    // Helper Method
    private ProductDto MapToDto(Product product)
    {
        var finalPrice = product.DiscountPercentage.HasValue
            ? product.Price - (product.Price * product.DiscountPercentage.Value / 100)
            : product.Price;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            OldPrice = product.Price,
            NewPrice = finalPrice,
            DiscountPercentage = product.DiscountPercentage,
            UnitType = product.UnitType.ToString(),
            IsAvailable = product.BranchInventories != null && product.BranchInventories.Any(bi => bi.Quantity > 0),
            Weight = product.Weight,
            WeightUnit = product.WeightUnit?.ToString(),
            Images = product.Images
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new ProductImageDto
            {
                Id = i.Id,
                Url = i.ImageUrl,
                DisplayOrder = i.DisplayOrder
            })
            .ToList(),
            CategoryName = product.Category?.Name,
            BrandName = product.Brand?.Name,
            AverageRating = product.Reviews != null && product.Reviews.Any()
            ? (double)product.Reviews.Average(r => r.Rating)
            : 0,
            ReviewsCount = product.Reviews?.Count ?? 0
        };
    }
}