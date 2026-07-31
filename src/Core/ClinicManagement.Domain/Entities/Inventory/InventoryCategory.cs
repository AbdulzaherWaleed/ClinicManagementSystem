using ClinicManagement.Domain.Common;

namespace ClinicManagement.Domain.Entities.Inventory;

public class InventoryCategory : BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>();
}