namespace API.Domain.Products;

public class Product {
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public int CategoryId { get; set; }
	public string CategoryName { get; set; } = string.Empty;
	public int BrandId { get; set; }
	public string BrandName { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int MinStock { get; set; }
	public bool Active { get; set; }
	public DateTime CreatedAt { get; set; }
	public int CurrentStock { get; set; }
}
