using Application.DTOs.Offer;



namespace Application.Interfaces.Services;

public interface IOfferService
{
    Task<IEnumerable<OfferDto>> GetAllOffersAsync(OfferFilter filtre);
    Task<IEnumerable<OfferDto>> GetActiveOffersAsync();
    Task<OfferDto?> GetOfferByIdAsync(int id);
    Task<OfferDto> CreateOfferAsync(CreateOfferDto dto);
    Task<bool> UpdateOfferAsync(UpdateOfferDto dto);
    Task<bool> DeleteOfferAsync(int id);
    Task<bool> AddProductToOfferAsync(int offerId, AddOfferProductDto dto);
    Task<bool> RemoveProductFromOfferAsync(int offerId, int productId);

    Task<bool> ToggleOfferStatusAsync(int id);
    Task<OfferStatsDto> GetOfferStatsAsync();
}