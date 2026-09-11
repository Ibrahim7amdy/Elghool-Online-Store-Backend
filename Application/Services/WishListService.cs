using Application.DTOs.WishList;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Helpers;

namespace Application.Services;

public class WishListService : IWishListService
{
    private readonly IWishListRepository _wishListRepository;
    private readonly IProductRepository _productRepository;

    public WishListService(
        IWishListRepository wishListRepository,
        IProductRepository productRepository)
    {
        _wishListRepository = wishListRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<WishListDto>> GetMyWishListAsync(int customerId)
    {
        var items = await _wishListRepository.GetByCustomerAsync(customerId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<bool> AddToWishListAsync(int customerId, int productId)
    {
        if (!await _productRepository.ExistsAsync(productId))
            throw new Exception("المنتج مش موجود.");

        if (await _wishListRepository.ExistsAsync(customerId, productId))
            throw new Exception("المنتج موجود في المفضلة بالفعل.");

        await _wishListRepository.AddAsync(new WishList
        {
            CustomerId = customerId,
            ProductId = productId,
            AddedAt = DateTime.UtcNow
        });

        return true;
    }

    public async Task<bool> RemoveFromWishListAsync(int customerId, int productId)
    {
        if (!await _wishListRepository.ExistsAsync(customerId, productId))
            return false;

        await _wishListRepository.RemoveAsync(customerId, productId);
        return true;
    }


    private static WishListDto MapToDto(WishList w)
    {
        var product = w.Product;
        var finalPrice = product.DiscountPercentage.HasValue
            ? product.Price - (product.Price * product.DiscountPercentage.Value / 100)
            : product.Price;

        return new WishListDto
        {
            ProductId = product.Id,
            ProductName = product.Name,
            OldPrice = product.Price,
            NewPrice = finalPrice,
            DiscountPercentage = product.DiscountPercentage,
            ProductImage = product.Images?.OrderBy(i => i.DisplayOrder)
                .FirstOrDefault()?.ImageUrl,
            IsAvailable = product.BranchInventories?.Any(bi => bi.Quantity > 0) ?? false,
            AddedAt = w.AddedAt.ToEgyptTime()
        };
    }
}