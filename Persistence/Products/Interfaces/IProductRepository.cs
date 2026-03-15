using API.Domain.Products;
using API.Persistence.Shared;

namespace API.Persistence.Products.Interfaces;

public interface IProductRepository {
	Task<PagedData<Product>> GetAllAsync(ProductsFilter filter);
	Task<Product?> GetByIdAsync(int id);
	Task<Product> CreateAsync(string name, string? description, int categoryId, int brandId, decimal price, int minStock);
	Task<Product?> UpdateAsync(int id, string name, string? description, int categoryId, int brandId, int minStock);
	Task<Product?> UpdatePriceAsync(int id, decimal price, string? reason);
	Task<bool> SetActiveAsync(int id, bool active);
	Task<bool> DeleteAsync(int id);
	Task<PagedData<PriceHistory>> GetPriceHistoryAsync(PriceHistoryFilter filter);
	Task<int> GetCurrentStockAsync(int productId);
}
