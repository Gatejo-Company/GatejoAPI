using API.DataAccess.Interfaces;
using API.Domain.Categories;
using API.Persistence.Categories.Interfaces;
using API.Persistence.Shared;
using API.Utils.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace API.Persistence.Categories.Repositories;

[Injectable(ServiceLifetime.Scoped)]
public class CategoryRepository : ICategoryRepository {
	private readonly ICConnection _connection;

	public CategoryRepository(ICConnection connection) {
		_connection = connection;
	}

	private static void Map(Category obj, ICDataReader rs) {
		obj.Id = rs.GetValue<int>("id");
		obj.Name = rs.GetValue<string>("name");
		obj.Description = rs.GetValue<string?>("description");
	}

	public async Task<PagedData<Category>> GetAllAsync(PagedFilter filter) {
		await _connection.Connect();
		return await PaginationHelper.FetchPagedAsync<Category>(
			_connection,
			"SELECT id, name, description, COUNT(*) OVER() AS total_count FROM categories ORDER BY id",
			null, Map, filter.Page, filter.PageSize);
	}

	public async Task<Category?> GetByIdAsync(int id) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "SELECT id, name, description FROM categories WHERE id = @id";
		cmd.AddParameter("id", id);
		return await cmd.ExecuteSelect<Category>(Map);
	}

	public async Task<Category> CreateAsync(string name, string? description) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = @"
            INSERT INTO categories (name, description)
            VALUES (@name, @description)
            RETURNING id, name, description";
		cmd.AddParameter("name", name);
		cmd.AddParameter("description", (object?)description ?? DBNull.Value);
		return (await cmd.ExecuteSelect<Category>(Map))!;
	}

	public async Task<Category?> UpdateAsync(int id, string name, string? description) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = @"
            UPDATE categories SET name = @name, description = @description
            WHERE id = @id
            RETURNING id, name, description";
		cmd.AddParameter("id", id);
		cmd.AddParameter("name", name);
		cmd.AddParameter("description", (object?)description ?? DBNull.Value);
		return await cmd.ExecuteSelect<Category>(Map);
	}

	public async Task<bool> DeleteAsync(int id) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "DELETE FROM categories WHERE id = @id";
		cmd.AddParameter("id", id);
		return await cmd.ExecuteCommandNonQuery();
	}
}
