using API.Domain.Users;
using API.Persistence.Shared;
using API.Persistence.Shared;

namespace API.Persistence.Users.Interfaces;

public interface IUserRepository {
	Task<PagedData<User>> GetAllAsync(PagedFilter filter);
	Task<User?> GetByIdAsync(int id);
	Task<User?> UpdateAsync(int id, string name, string email, int roleId);
	Task<bool> SetActiveAsync(int id, bool active);
	Task<bool> DeleteAsync(int id);
}
