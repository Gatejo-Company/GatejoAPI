namespace API.Domain.SaleInvoices;

public class SaleInvoice {
	public int Id { get; set; }
	public DateOnly Date { get; set; }
	public decimal Total { get; set; }
	public bool OnCredit { get; set; }
	public bool Reversed { get; set; }
	public DateTime? PaidAt { get; set; }
	public string? Notes { get; set; }
	public DateTime CreatedAt { get; set; }
	public List<SaleItem> Items { get; set; } = [];
}
