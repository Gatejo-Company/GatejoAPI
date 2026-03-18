using API.Domain.Categories;
using API.Persistence.Shared;

namespace API.Persistence.Categories.Interfaces;

public interface ICategoryRepository {
	Task<PagedData<Category>> GetAllAsync(PagedFilter filter);
	Task<Category?> GetByIdAsync(int id);
	Task<Category> CreateAsync(string name, string? description);
	Task<Category?> UpdateAsync(int id, string name, string? description);
	Task<bool> DeleteAsync(int id);
}
