namespace API.Domain.Products;

public class PriceHistory {
	public int Id { get; set; }
	public int ProductId { get; set; }
	public decimal Price { get; set; }
	public string? Reason { get; set; }
	public DateTime CreatedAt { get; set; }
}
