using System.ComponentModel.DataAnnotations;

namespace API.API_Clean_Architecture.Controllers.Products;

public class UpdatePriceRequest {
	[Required, Range(0.01, double.MaxValue)]
	public decimal Price { get; set; }
	public string? Reason { get; set; }
}
