using Application.DTOs.Order;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Helpers;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IBranchInventoryRepository _inventoryRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IOfferRepository _offerRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IBranchInventoryRepository inventoryRepository,
        IBranchRepository branchRepository,
        ICustomerRepository customerRepository,
        ICartRepository cartRepository,
        IOfferRepository offerRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
        _branchRepository = branchRepository;
        _customerRepository = customerRepository;
        _cartRepository = cartRepository;
        _offerRepository = offerRepository;
    }

    public async Task<OrderDto> CreateOrderAsync(int customerId, CreateOrderDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        var branchId = dto.BranchId ?? customer?.PreferredBranchId
            ?? throw new Exception("لازم تحدد فرع للاستلام.");

        if (!await _branchRepository.ExistsAsync(branchId))
            throw new Exception("الفرع مش موجود.");

        var orderItems = new List<OrderItem>();
        decimal totalPrice = 0;

        // =======================================================
        // المسار الأول: عرض الباقة (Bundle Track)
        // =======================================================
        if (dto.OfferId.HasValue)
        {
            var offer = await _offerRepository.GetByIdWithProductsAsync(dto.OfferId.Value);
            var now = DateTime.UtcNow;

            if (offer == null || !offer.IsActive || offer.StartDate > now || offer.EndDate < now)
                throw new Exception("هذا العرض غير متاح حالياً أو انتهت صلاحيته.");

            if (!offer.BundlePrice.HasValue)
                throw new Exception("هذا العرض ليس باقة مجمعة.");

            // إجمالي الفاتورة هو سعر الباقة بالكامل
            totalPrice = offer.BundlePrice.Value;

            foreach (var offerProduct in offer.OfferProducts)
            {
                var product = offerProduct.Product;
                if (product == null) continue;

                var inventory = await _inventoryRepository
                    .GetByProductAndBranchAsync(offerProduct.ProductId, branchId);

                if (inventory == null || inventory.Quantity < offerProduct.Quantity)
                    throw new Exception($"المنتج {product.Name} داخل الباقة غير متوفر بالكمية المطلوبة في هذا الفرع.");

                orderItems.Add(new OrderItem
                {
                    ProductId = offerProduct.ProductId,
                    Quantity = offerProduct.Quantity,

                    UnitPrice = 0
                });

                // سحب الكمية من المخزن
                inventory.Quantity -= offerProduct.Quantity;
                await _inventoryRepository.UpdateAsync(inventory);
            }
        }
        // =======================================================
        // 🟢 المسار الثاني: مسار السلة العادي والخصومات المئوية (Cart Track)
        // =======================================================
        else
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new Exception("السلة فارغة، مفيش منتجات لطلبها.");

            foreach (var item in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"المنتج رقم {item.ProductId} مش موجود.");

                var inventory = await _inventoryRepository
                    .GetByProductAndBranchAsync(item.ProductId, branchId);

                if (inventory == null || inventory.Quantity < item.Quantity)
                    throw new Exception($"المنتج {product.Name} مش متاح بالكمية المطلوبة في الفرع ده.");

                decimal unitPrice = product.Price;

                // البحث عن عرض خصم مئوي نشط للمنتج للعمل Override
                var activeOffer = await _offerRepository.GetActivePercentageOfferForProductAsync(item.ProductId);

                if (activeOffer != null && activeOffer.DiscountPercentage.HasValue)
                {
                    unitPrice = product.Price - (product.Price * activeOffer.DiscountPercentage.Value / 100);
                }
                else if (product.DiscountPercentage.HasValue)
                {
                    unitPrice = product.Price - (product.Price * product.DiscountPercentage.Value / 100);
                }

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = Math.Round(unitPrice, 2)
                });

                totalPrice += unitPrice * item.Quantity;

                // سحب الكمية من المخزن
                inventory.Quantity -= item.Quantity;
                await _inventoryRepository.UpdateAsync(inventory);
            }

            // تفريغ السلة في حالة الشراء العادي فقط
            await _cartRepository.ClearAsync(customerId);
        }

        // =======================================================
        // حفظ الأوردر النهائي
        // =======================================================
        var order = new Order
        {
            OrderNumber = await _orderRepository.GenerateOrderNumberAsync(),
            CustomerId = customerId,
            BranchId = branchId,
            Notes = dto.Notes,
            OfferId=dto.OfferId,
            TotalPrice = Math.Round(totalPrice, 2),
            Status = OrderStatus.Pending,
            OrderItems = orderItems
        };

        await _orderRepository.AddAsync(order);

        var createdOrder = await _orderRepository.GetByIdAsync(order.Id);
        return MapToDto(createdOrder!);
    }






    public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int customerId)
    {
        var orders = await _orderRepository.GetByCustomerAsync(customerId);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int orderId, int customerId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.CustomerId != customerId) return null;
        return MapToDto(order);
    }

    public async Task<bool> CancelOrderAsync(int orderId, int customerId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.CustomerId != customerId) return false;

        if (order.Status != OrderStatus.Pending)
            throw new Exception("مش ممكن تلغي الأوردر ده، اتأكد من حالته الحالية.");


        await ReturnInventoryAsync(order.OrderItems, order.BranchId);


        await _orderRepository.DeleteAsync(order);
        return true;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByBranchAsync(int branchId)
    {
        var orders = await _orderRepository.GetByBranchAsync(branchId);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDto?> GetOrderByIdAdminAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        return order == null ? null : MapToDto(order);
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) return false;

        if (order.Status == OrderStatus.PickedUp)
            throw new Exception("الأوردر ده اتسلم بالفعل.");

        order.Status = dto.Status;
        await _orderRepository.UpdateAsync(order);
        return true;
    }

    public async Task<bool> CancelOrderByAdminAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) return false;

        if (order.Status == OrderStatus.PickedUp)
            throw new Exception("الأوردر ده اتسلم بالفعل مش ممكن تلغيه.");


        await ReturnInventoryAsync(order.OrderItems, order.BranchId);
        await _orderRepository.DeleteAsync(order);
        return true;
    }

    private async Task ReturnInventoryAsync(IEnumerable<OrderItem> orderItems, int branchId)
    {
        foreach (var item in orderItems)
        {
            var inventory = await _inventoryRepository
                .GetByProductAndBranchAsync(item.ProductId, branchId);
            if (inventory != null)
            {
                inventory.Quantity += item.Quantity;
                await _inventoryRepository.UpdateAsync(inventory);
            }
        }
    }

    private OrderDto MapToDto(Order order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        Status = order.Status.ToString(),
        TotalPrice = order.TotalPrice,
        Notes = order.Notes,
        CreatedAt = order.CreatedAt.ToEgyptTime(),
        BranchName = order.Branch?.Name ?? string.Empty,
        CustomerName = $"{order.Customer?.FirstName} {order.Customer?.LastName}".Trim(),
        Items = order.OrderItems.Select(oi => new OrderItemDto
        {
            ProductId = oi.ProductId,
            ProductName = oi.Product?.Name ?? string.Empty,
            ProductImage = oi.Product?.Images?.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.ImageUrl,
            Quantity = oi.Quantity,

            UnitPrice = oi.UnitPrice == 0 ? null : oi.UnitPrice,
            Subtotal = oi.UnitPrice == 0 ? null : (oi.UnitPrice * oi.Quantity)
        }).ToList()
    };
}