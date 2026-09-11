using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IOfferRepository
{
    Task<IEnumerable<Offer>> GetAllAsync();
    Task<IEnumerable<Offer>> GetActiveAsync();
    Task<Offer?> GetByIdAsync(int id);
    Task AddAsync(Offer offer);
    Task UpdateAsync(Offer offer);
    Task DeleteAsync(Offer offer);
    Task<bool> ExistsAsync(int id);


    Task<int> GetOrdersCountByOfferAsync(int offerId);
    Task<Dictionary<int, int>> GetOrdersCountForAllOffersAsync();

    Task<Offer?> GetByIdWithProductsAsync(int offerId);
    Task<Offer?> GetActivePercentageOfferForProductAsync(int productId);
}