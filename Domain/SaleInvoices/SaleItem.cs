namespace API.Domain.SaleInvoices;

public class SaleItem {
	public int Id { get; set; }
	public int SaleInvoiceId { get; set; }
	public int ProductId { get; set; }
	public string ProductName { get; set; } = string.Empty;
	public int Quantity { get; set; }
	public decimal UnitPrice { get; set; }
	public decimal Subtotal { get; set; }
}
