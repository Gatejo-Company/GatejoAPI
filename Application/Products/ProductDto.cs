using API.Domain.Products;

namespace API.Application.Products;

public record ProductDto(
	int Id,
	string Name,
	string? Description,
	int CategoryId,
	string CategoryName,
	int BrandId,
	string BrandName,
	decimal Price,
	int MinStock,
	bool Active,
	DateTime CreatedAt,
	int CurrentStock) {
	public static ProductDto From(Product p) => new(
		p.Id, p.Name, p.Description,
		p.CategoryId, p.CategoryName,
		p.BrandId, p.BrandName,
		p.Price, p.MinStock, p.Active,
		p.CreatedAt, p.CurrentStock);
}
