using API.DataAccess.Interfaces;
using API.Domain.Users;
using API.Persistence.Shared;
using API.Persistence.Users.Interfaces;
using API.Utils.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace API.Persistence.Users.Repositories;

[Injectable(ServiceLifetime.Scoped)]
public class UserRepository : IUserRepository {
	private readonly ICConnection _connection;

	public UserRepository(ICConnection connection) {
		_connection = connection;
	}

	private static void Map(User obj, ICDataReader rs) {
		obj.Id = rs.GetValue<int>("id");
		obj.Name = rs.GetValue<string>("name");
		obj.Email = rs.GetValue<string>("email");
		obj.RoleId = rs.GetValue<int>("role_id");
		obj.RoleName = rs.GetValue<string>("role_name");
		obj.Active = rs.GetValue<bool>("active");
		obj.CreatedAt = rs.GetValue<DateTime>("created_at");
	}

	private const string SelectSql = @"
        SELECT u.id, u.name, u.email, u.role_id, r.name AS role_name, u.active, u.created_at
        FROM users u
        INNER JOIN roles r ON r.id = u.role_id";

	public async Task<PagedData<User>> GetAllAsync(PagedFilter filter) {
		await _connection.Connect();
		return await PaginationHelper.FetchPagedAsync<User>(
			_connection,
			@"SELECT u.id, u.name, u.email, u.role_id, r.name AS role_name, u.active, u.created_at,
			         COUNT(*) OVER() AS total_count
			  FROM users u
			  INNER JOIN roles r ON r.id = u.role_id
			  ORDER BY u.id",
			null, Map, filter.Page, filter.PageSize);
	}

	public async Task<User?> GetByIdAsync(int id) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = SelectSql + " WHERE u.id = @id";
		cmd.AddParameter("id", id);
		return await cmd.ExecuteSelect<User>(Map);
	}

	public async Task<User?> UpdateAsync(int id, string name, string email, int roleId) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = @"
            WITH upd AS (
                UPDATE users SET name = @name, email = @email, role_id = @roleId
                WHERE id = @id
                RETURNING id
            )
            SELECT u.id, u.name, u.email, u.role_id, r.name AS role_name, u.active, u.created_at
            FROM users u
            INNER JOIN roles r ON r.id = u.role_id
            WHERE u.id = (SELECT id FROM upd)";
		cmd.AddParameter("id", id);
		cmd.AddParameter("name", name);
		cmd.AddParameter("email", email);
		cmd.AddParameter("roleId", roleId);
		return await cmd.ExecuteSelect<User>(Map);
	}

	public async Task<bool> SetActiveAsync(int id, bool active) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "UPDATE users SET active = @active WHERE id = @id";
		cmd.AddParameter("id", id);
		cmd.AddParameter("active", active);
		return await cmd.ExecuteCommandNonQuery();
	}

	public async Task<bool> DeleteAsync(int id) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "DELETE FROM users WHERE id = @id";
		cmd.AddParameter("id", id);
		return await cmd.ExecuteCommandNonQuery();
	}
}
