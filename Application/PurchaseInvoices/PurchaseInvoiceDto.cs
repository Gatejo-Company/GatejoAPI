using API.Domain.PurchaseInvoices;

namespace API.Application.PurchaseInvoices;

public record PurchaseItemDto(int Id, int ProductId, string ProductName, int Quantity, decimal UnitPrice) {
	public static PurchaseItemDto From(PurchaseItem item) => new(item.Id, item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
}

public record PurchaseInvoiceDto(
	int Id,
	int SupplierId,
	string SupplierName,
	DateOnly Date,
	decimal Total,
	decimal Paid,
	string? Notes,
	DateTime CreatedAt,
	List<PurchaseItemDto> Items) {
	public static PurchaseInvoiceDto From(PurchaseInvoice inv) => new(
		inv.Id, inv.SupplierId, inv.SupplierName,
		inv.Date, inv.Total, inv.Paid,
		inv.Notes, inv.CreatedAt,
		inv.Items.Select(PurchaseItemDto.From).ToList());
}
