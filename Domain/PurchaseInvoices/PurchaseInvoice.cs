namespace API.Domain.PurchaseInvoices;

public class PurchaseInvoice {
	public int Id { get; set; }
	public int SupplierId { get; set; }
	public string SupplierName { get; set; } = string.Empty;
	public DateOnly Date { get; set; }
	public decimal Total { get; set; }
	public decimal Paid { get; set; }
	public string? Notes { get; set; }
	public DateTime CreatedAt { get; set; }
	public List<PurchaseItem> Items { get; set; } = [];
}
