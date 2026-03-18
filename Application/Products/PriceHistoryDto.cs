using API.Domain.Products;

namespace API.Application.Products;

public record PriceHistoryDto(int Id, int ProductId, decimal Price, string? Reason, DateTime CreatedAt) {
	public static PriceHistoryDto From(PriceHistory ph) => new(ph.Id, ph.ProductId, ph.Price, ph.Reason, ph.CreatedAt);
}
