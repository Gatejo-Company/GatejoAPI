using System.ComponentModel.DataAnnotations;

namespace API.API_Clean_Architecture.Controllers.PurchaseInvoices;

public class InvoiceItemRequest {
	[Required]
	public int ProductId { get; set; }
	[Required, Range(1, int.MaxValue)]
	public int Quantity { get; set; }
	[Required, Range(0.01, double.MaxValue)]
	public decimal UnitPrice { get; set; }
}

public class CreatePurchaseInvoiceRequest {
	[Required]
	public int SupplierId { get; set; }
	[Required]
	public DateOnly Date { get; set; }
	public string? Notes { get; set; }
	[Required, MinLength(1)]
	public List<InvoiceItemRequest> Items { get; set; } = [];
}
