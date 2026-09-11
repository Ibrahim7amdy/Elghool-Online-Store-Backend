using Domain.Enums;

namespace Application.DTOs.BranchInventory;

public class StockHistoryDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? SourceBranchName { get; set; }
    public string? DestinationBranchName { get; set; }
    public int Quantity { get; set; }
    public TransactionType Type { get; set; }  // Addition, Subtraction, Transfer
    public DateTime TransactionDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}