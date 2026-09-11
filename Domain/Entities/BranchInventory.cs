namespace Domain.Entities;

public class BranchInventory
{
    public int BranchId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int LowStockThreshold { get; set; } = 10;
    public Product Product { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
}