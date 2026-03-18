namespace API.Persistence.Products.Interfaces;

public record ProductsFilter(int? CategoryId, int? BrandId, bool? Active, bool? LowStock, int Page, int PageSize);
