using System.ComponentModel.DataAnnotations;

namespace API.API_Clean_Architecture.Controllers.Products;

public class CreateProductRequest {
	[Required, MaxLength(200)]
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	[Required]
	public int CategoryId { get; set; }
	[Required]
	public int BrandId { get; set; }
	[Required, Range(0.01, double.MaxValue)]
	public decimal Price { get; set; }
	[Range(0, int.MaxValue)]
	public int MinStock { get; set; }
}
