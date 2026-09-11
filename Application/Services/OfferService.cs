using Application.DTOs.Offer;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _offerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IFileService _fileService;

    public OfferService(
        IOfferRepository offerRepository,
        IProductRepository productRepository,
        IFileService fileService)
    {
        _offerRepository = offerRepository;
        _productRepository = productRepository;
        _fileService = fileService;
    }


    public async Task<IEnumerable<OfferDto>> GetAllOffersAsync(OfferFilter filter)
    {
        var offers = await _offerRepository.GetAllAsync();
        var requestsCounts = await _offerRepository.GetOrdersCountForAllOffersAsync();


        var offerDtos = offers.Select(o => MapToDto(o, requestsCounts.GetValueOrDefault(o.Id))).AsQueryable();


        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var search = filter.SearchTerm.ToLower();
            offerDtos = offerDtos.Where(o => o.Title.ToLower().Contains(search));
        }


        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            offerDtos = offerDtos.Where(o => o.Status.Equals(filter.Status, StringComparison.OrdinalIgnoreCase));
        }


        if (!string.IsNullOrWhiteSpace(filter.OfferType))
        {
            if (filter.OfferType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
                offerDtos = offerDtos.Where(o => o.DiscountPercentage.HasValue);
            else if (filter.OfferType.Equals("Bundle", StringComparison.OrdinalIgnoreCase))
                offerDtos = offerDtos.Where(o => o.BundlePrice.HasValue);
        }


        offerDtos = filter.SortBy switch
        {
            "PriceDesc" => offerDtos.OrderByDescending(o => o.TotalOfferPrice),
            "PriceAsc" => offerDtos.OrderBy(o => o.TotalOfferPrice),
            "RequestsDesc" => offerDtos.OrderByDescending(o => o.RequestsCount),
            "RequestsAsc" => offerDtos.OrderBy(o => o.RequestsCount),
            _ => offerDtos.OrderByDescending(o => o.StartDate)
        };

        return offerDtos.ToList();
    }


    public async Task<IEnumerable<OfferDto>> GetActiveOffersAsync()
    {
        var offers = await _offerRepository.GetActiveAsync();
        var requestsCounts = await _offerRepository.GetOrdersCountForAllOffersAsync();
        return offers.Select(o => MapToDto(o, requestsCounts.GetValueOrDefault(o.Id))).ToList();
    }

    public async Task<OfferDto?> GetOfferByIdAsync(int id)
    {
        var offer = await _offerRepository.GetByIdAsync(id);
        if (offer == null) return null;
        var requestsCount = await _offerRepository.GetOrdersCountByOfferAsync(id);
        return MapToDto(offer, requestsCount);
    }

    public async Task<OfferDto> CreateOfferAsync(CreateOfferDto dto)
    {
        string? imageUrl = null;
        if (dto.Image != null)
            imageUrl = await _fileService.UploadFileAsync(dto.Image, "offers");

        var offer = new Offer
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = imageUrl,
            DiscountPercentage = dto.DiscountPercentage,
            BundlePrice = dto.BundlePrice,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = (DateTime.UtcNow >= dto.StartDate && DateTime.UtcNow < dto.EndDate) ? true : false
        };

        await _offerRepository.AddAsync(offer);
        return MapToDto(offer, 0);
    }

    public async Task<bool> UpdateOfferAsync(UpdateOfferDto dto)
    {
        var offer = await _offerRepository.GetByIdAsync(dto.Id);
        if (offer == null) return false;

        if (dto.Image != null)
        {
            if (!string.IsNullOrEmpty(offer.ImageUrl))
                await _fileService.DeleteFileAsync(offer.ImageUrl);

            offer.ImageUrl = await _fileService.UploadFileAsync(dto.Image, "offers");
        }

        offer.Title = dto.Title;
        offer.Description = dto.Description;
        offer.DiscountPercentage = dto.DiscountPercentage;
        offer.BundlePrice = dto.BundlePrice;
        offer.StartDate = dto.StartDate;
        offer.EndDate = dto.EndDate;
        offer.IsActive = (DateTime.UtcNow >= dto.StartDate && DateTime.UtcNow < dto.EndDate) ? true : false;

        await _offerRepository.UpdateAsync(offer);
        return true;
    }


    public async Task<bool> ToggleOfferStatusAsync(int id)
    {
        var offer = await _offerRepository.GetByIdAsync(id);
        if (offer == null) return false;

        offer.IsActive = !offer.IsActive;
        await _offerRepository.UpdateAsync(offer);
        return true;
    }

    public async Task<bool> DeleteOfferAsync(int id)
    {
        var offer = await _offerRepository.GetByIdAsync(id);
        if (offer == null) return false;

        if (!string.IsNullOrEmpty(offer.ImageUrl))
            await _fileService.DeleteFileAsync(offer.ImageUrl);

        await _offerRepository.DeleteAsync(offer);
        return true;
    }

    public async Task<bool> AddProductToOfferAsync(int offerId, AddOfferProductDto dto)
    {
        var offer = await _offerRepository.GetByIdAsync(offerId);
        if (offer == null) return false;

        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null) return false;

        if (offer.OfferProducts.Any(op => op.ProductId == dto.ProductId))
            return false;

        product.DiscountPercentage = offer.DiscountPercentage;

        offer.OfferProducts.Add(new OfferProduct
        {
            OfferId = offerId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity
        });

        await _offerRepository.UpdateAsync(offer);
        return true;
    }

    public async Task<bool> RemoveProductFromOfferAsync(int offerId, int productId)
    {
        var offer = await _offerRepository.GetByIdAsync(offerId);
        if (offer == null) return false;

        var offerProduct = offer.OfferProducts.FirstOrDefault(op => op.ProductId == productId);
        if (offerProduct == null) return false;
        
        var product= await _productRepository.GetByIdAsync(productId);
        if(product == null) return false;
        product.DiscountPercentage = null;

        offer.OfferProducts.Remove(offerProduct);
        await _offerRepository.UpdateAsync(offer);
        return true;
    }

    public async Task<OfferStatsDto> GetOfferStatsAsync()
    {
        var offers = (await _offerRepository.GetAllAsync()).ToList();
        var requestsCounts = await _offerRepository.GetOrdersCountForAllOffersAsync();
        var now = DateTime.UtcNow;

        var activeNow = offers.Count(o => o.IsActive && o.StartDate <= now && o.EndDate >= now);


        var endedOffers = offers.Where(o => o.EndDate < now).ToList();
        var endedWithRequests = endedOffers.Count(o => requestsCounts.GetValueOrDefault(o.Id) > 0);
        var successRate = endedOffers.Count == 0 ? 0
            : Math.Round((decimal)endedWithRequests / endedOffers.Count * 100, 1);

        var totalProducts = offers.SelectMany(o => o.OfferProducts).Select(op => op.ProductId).Distinct().Count();

        var topRequested = offers
        .Select(o => new TopRequestedOfferDto
        {
            Title = o.Title,
            RequestsCount = requestsCounts.GetValueOrDefault(o.Id)
        })
        .OrderByDescending(o => o.RequestsCount)
        .Take(5)
        .ToList();

        var topOffer = topRequested.FirstOrDefault();
        var hasAnyRequests = topOffer != null && topOffer.RequestsCount > 0;

        var typeDistribution = new List<OfferTypeDistributionDto>
    {
        new() { Type = "Percentage", Count = offers.Count(o => o.DiscountPercentage.HasValue) },
        new() { Type = "Bundle", Count = offers.Count(o => o.BundlePrice.HasValue) }
    };

        return new OfferStatsDto
        {
            TotalOffers = offers.Count,
            ActiveNow = activeNow,
            SuccessRate = successRate,
            TotalProductsInOffers = totalProducts,
            MostRequestedOfferTitle = hasAnyRequests ? topOffer!.Title : "لا توجد طلبات بعد",
            MostRequestedCount = hasAnyRequests ? topOffer!.RequestsCount : 0,
            TypeDistribution = typeDistribution,
            TopRequestedOffers = topRequested
        };
    }



    private OfferDto MapToDto(Offer offer, int requestsCount)
    {
        var now = DateTime.UtcNow;
        var status = offer.EndDate < now ? "Ended"
            : offer.StartDate > now ? "Upcoming"
            : !offer.IsActive ? "Stopped"
            : "Active";


        decimal totalOfferPrice = 0;
        if (offer.BundlePrice.HasValue)
        {
            totalOfferPrice = offer.BundlePrice.Value;
        }
        else if (offer.DiscountPercentage.HasValue)
        {
            var discount = offer.DiscountPercentage.Value;

            totalOfferPrice = offer.OfferProducts.Sum(op => op.Product.Price - (op.Product.Price * discount / 100));
        }

        return new OfferDto
        {
            Id = offer.Id,
            Title = offer.Title,
            Description = offer.Description,
            ImageUrl = offer.ImageUrl,
            DiscountPercentage = offer.DiscountPercentage,
            BundlePrice = offer.BundlePrice,
            StartDate = offer.StartDate,
            EndDate = offer.EndDate,
            IsActive = offer.IsActive,
            Status = status,
            RequestsCount = requestsCount,
            TotalOfferPrice = totalOfferPrice,
            Products = offer.OfferProducts.Select(op => new OfferProductDto
            {
                ProductId = op.ProductId,
                ProductName = op.Product.Name,
                ProductImage = op.Product.Images.FirstOrDefault()?.ImageUrl,
                OriginalPrice = op.Product.Price,
                FinalPrice = offer.DiscountPercentage.HasValue
                ? op.Product.Price - (op.Product.Price * offer.DiscountPercentage.Value / 100)
                : op.Product.Price,
                Quantity = op.Quantity
            }).ToList()
        };
    }
}