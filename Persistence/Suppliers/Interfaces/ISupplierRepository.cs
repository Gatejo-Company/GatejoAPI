using API.Domain.Suppliers;
using API.Persistence.Shared;

namespace API.Persistence.Suppliers.Interfaces;

public interface ISupplierRepository {
	Task<PagedData<Supplier>> GetAllAsync(PagedFilter filter);
	Task<Supplier?> GetByIdAsync(int id);
	Task<Supplier> CreateAsync(string name, string? phone, string? email, string? notes);
	Task<Supplier?> UpdateAsync(int id, string name, string? phone, string? email, string? notes);
	Task<bool> DeleteAsync(int id);
}
