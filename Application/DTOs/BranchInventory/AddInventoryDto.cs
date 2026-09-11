namespace Application.DTOs.BranchInventory;

public class AddInventoryDto
{
    public int BranchId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int? LowStockThreshold { get; set; }
    public string Notes { get; set; } = string.Empty;
}