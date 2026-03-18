using API.Domain.Brands;
using API.Persistence.Shared;

namespace API.Persistence.Brands.Interfaces;

public interface IBrandRepository {
	Task<PagedData<Brand>> GetAllAsync(PagedFilter filter);
	Task<Brand?> GetByIdAsync(int id);
	Task<Brand> CreateAsync(string name);
	Task<Brand?> UpdateAsync(int id, string name);
	Task<bool> DeleteAsync(int id);
}
