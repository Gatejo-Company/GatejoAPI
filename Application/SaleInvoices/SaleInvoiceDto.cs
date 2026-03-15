using API.Domain.SaleInvoices;

namespace API.Application.SaleInvoices;

public record SaleLineItemDto(int Id, int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal Subtotal) {
	public static SaleLineItemDto From(SaleItem item) => new(item.Id, item.ProductId, item.ProductName, item.Quantity, item.UnitPrice, item.Subtotal);
}

public record SaleInvoiceDto(
	int Id,
	DateOnly Date,
	decimal Total,
	bool OnCredit,
	bool Reversed,
	DateTime? PaidAt,
	string? Notes,
	DateTime CreatedAt,
	List<SaleLineItemDto> Items) {
	public static SaleInvoiceDto From(SaleInvoice inv) => new(
		inv.Id, inv.Date, inv.Total,
		inv.OnCredit, inv.Reversed, inv.PaidAt,
		inv.Notes, inv.CreatedAt,
		inv.Items.Select(SaleLineItemDto.From).ToList());
}
