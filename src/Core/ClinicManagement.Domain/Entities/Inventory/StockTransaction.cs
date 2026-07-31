using ClinicManagement.Domain.Common;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Entities.Inventory;

public class StockTransaction : BaseAuditableEntity
{
    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = default!;

    public StockTransactionType Type { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public Guid? PerformedByUserId { get; set; }
}