using API.DataAccess.Interfaces;
using API.Domain.Roles;
using API.Persistence.Roles.Interfaces;
using API.Persistence.Shared;
using API.Utils.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace API.Persistence.Roles.Repositories;

[Injectable(ServiceLifetime.Scoped)]
public class RoleRepository : IRoleRepository {
	private readonly ICConnection _connection;

	public RoleRepository(ICConnection connection) {
		_connection = connection;
	}

	private static void Map(Role obj, ICDataReader rs) {
		obj.Id = rs.GetValue<int>("id");
		obj.Name = rs.GetValue<string>("name");
		obj.Description = rs.GetValue<string?>("description");
	}

	public async Task<PagedData<Role>> GetAllAsync(PagedFilter filter) {
		await _connection.Connect();
		return await PaginationHelper.FetchPagedAsync<Role>(
			_connection,
			"SELECT id, name, description, COUNT(*) OVER() AS total_count FROM roles ORDER BY id",
			null, Map, filter.Page, filter.PageSize);
	}

	public async Task<Role?> GetByIdAsync(int id) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "SELECT id, name, description FROM roles WHERE id = @id";
		cmd.AddParameter("id", id);
		return await cmd.ExecuteSelect<Role>(Map);
	}

}
