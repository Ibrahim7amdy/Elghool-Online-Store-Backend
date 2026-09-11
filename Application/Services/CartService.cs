using Application.DTOs.Cart;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
    }

    public async Task<CartDto> GetCartAsync(int customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        var items = await _cartRepository.GetByCustomerAsync(customerId);

        var cartItems = items.Select(item => MapToDto(item, customer?.PreferredBranchId)).ToList();

        return new CartDto
        {
            Items = cartItems,
            TotalItems = cartItems.Sum(i => i.Quantity),
            TotalPrice = cartItems.Sum(i => i.Subtotal)
        };
    }

    public async Task<bool> AddToCartAsync(int customerId, AddToCartDto dto)
    {
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
            throw new Exception("المنتج مش موجود.");

        if (!product.IsActive)
            throw new Exception("المنتج مش متاح.");

        var existingItem = await _cartRepository.GetItemAsync(customerId, dto.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            await _cartRepository.UpdateAsync(existingItem);
        }
        else
        {
            await _cartRepository.AddAsync(new CartItem
            {
                CustomerId = customerId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                AddedAt = DateTime.UtcNow
            });
        }

        return true;
    }

    public async Task<bool> UpdateCartItemAsync(int customerId, int productId, UpdateCartItemDto dto)
    {
        var item = await _cartRepository.GetItemAsync(customerId, productId);
        if (item == null) return false;

        item.Quantity = dto.Quantity;
        await _cartRepository.UpdateAsync(item);
        return true;
    }

    public async Task<bool> RemoveFromCartAsync(int customerId, int productId)
    {
        var item = await _cartRepository.GetItemAsync(customerId, productId);
        if (item == null) return false;

        await _cartRepository.RemoveAsync(item);
        return true;
    }

    public async Task ClearCartAsync(int customerId)
        => await _cartRepository.ClearAsync(customerId);

    private static CartItemDto MapToDto(CartItem item, int? preferredBranchId)
    {
        var product = item.Product;
        var finalPrice = product.DiscountPercentage.HasValue
            ? product.Price - (product.Price * product.DiscountPercentage.Value / 100)
            : product.Price;

        return new CartItemDto
        {
            ProductId = product.Id,
            ProductName = product.Name,
            UnitPrice = product.Price,
            FinalPrice = finalPrice,
            DiscountPercentage = product.DiscountPercentage,
            Quantity = item.Quantity,
            Subtotal = finalPrice * item.Quantity,
            ProductImage = product.Images?.OrderBy(i => i.DisplayOrder)
                .FirstOrDefault()?.ImageUrl,
            IsAvailable = preferredBranchId.HasValue
                ? product.BranchInventories?.Any(bi =>
                    bi.BranchId == preferredBranchId.Value &&
                    bi.Quantity >= item.Quantity) ?? false
                : product.BranchInventories?.Any(bi => bi.Quantity > 0) ?? false
        };
    }
}