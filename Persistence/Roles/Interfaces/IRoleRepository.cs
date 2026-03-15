using API.Domain.Roles;
using API.Persistence.Shared;

namespace API.Persistence.Roles.Interfaces;

public interface IRoleRepository {
	Task<PagedData<Role>> GetAllAsync(PagedFilter filter);
	Task<Role?> GetByIdAsync(int id);
}
