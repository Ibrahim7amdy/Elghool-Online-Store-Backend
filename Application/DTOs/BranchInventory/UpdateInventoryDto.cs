namespace Application.DTOs.BranchInventory;

public class UpdateInventoryDto
{
    public int Quantity { get; set; }
    public int? LowStockThreshold { get; set; } 
    public string Notes { get; set; } = string.Empty;

}