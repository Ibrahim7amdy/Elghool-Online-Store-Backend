using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IStockTransactionRepository
    {
        Task AddAsync(StockTransaction transaction);
        Task<List<StockTransaction>> GetHistoryAsync(int? productId, int? branchId);
    }
}
