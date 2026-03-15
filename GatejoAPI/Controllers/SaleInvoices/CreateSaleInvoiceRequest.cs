using System.ComponentModel.DataAnnotations;

namespace API.API_Clean_Architecture.Controllers.SaleInvoices;

public class SaleItemRequest {
	[Required]
	public int ProductId { get; set; }

	public int Quantity { get; set; }
	
	public decimal UnitPrice { get; set; }
}

public class CreateSaleInvoiceRequest {
    public int IdReversedInvoice { get; set; }
    [Required]
	public DateOnly Date { get; set; }
	public bool OnCredit { get; set; }
	public bool Reversed { get; set; }
	public string? Notes { get; set; }
	[Required, MinLength(1)]
	public List<SaleItemRequest> Items { get; set; } = [];
}
