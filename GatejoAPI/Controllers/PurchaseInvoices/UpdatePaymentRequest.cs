using System.ComponentModel.DataAnnotations;

namespace API.API_Clean_Architecture.Controllers.PurchaseInvoices;

public class UpdatePaymentRequest {
	[Required, Range(0, double.MaxValue)]
	public decimal Paid { get; set; }
}
