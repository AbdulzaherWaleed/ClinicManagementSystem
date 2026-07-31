using ClinicManagement.Domain.Common;

namespace ClinicManagement.Domain.Entities.Inventory;

public class InventoryItem : BaseAuditableEntity
{
    public Guid? DoctorId { get; set; }
    public Guid CategoryId { get; set; }
    public InventoryCategory Category { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string? Sku { get; set; }
    public string? Unit { get; set; }
    public int QuantityOnHand { get; set; }
    public int ReorderThreshold { get; set; }
    public decimal? UnitCost { get; set; }

    public ICollection<StockTransaction> Transactions { get; set; } = new List<StockTransaction>();
}