using API.DataAccess.Interfaces;
using API.Domain.Brands;
using API.Persistence.Brands.Interfaces;
using API.Persistence.Shared;
using API.Utils.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace API.Persistence.Brands.Repositories;

[Injectable(ServiceLifetime.Scoped)]
public class BrandRepository : IBrandRepository {
	private readonly ICConnection _connection;

	public BrandRepository(ICConnection connection) {
		_connection = connection;
	}

	private static void Map(Brand obj, ICDataReader rs) {
		obj.Id = rs.GetValue<int>("id");
		obj.Name = rs.GetValue<string>("name");
	}

	public async Task<PagedData<Brand>> GetAllAsync(PagedFilter filter) {
		
		return await PaginationHelper.FetchPagedAsync<Brand>(
			_connection,
			"SELECT id, name, COUNT(*) OVER() AS total_count FROM brands ORDER BY id",
			null, Map, filter.Page, filter.PageSize);
	}

	public async Task<Brand?> GetByIdAsync(int id) {
		
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "SELECT id, name FROM brands WHERE id = @id";
		cmd.AddParameter("id", id);
		return await cmd.ExecuteSelect<Brand>(Map);
	}

	public async Task<Brand> CreateAsync(string name) {
		
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "INSERT INTO brands (name) VALUES (@name) RETURNING id, name";
		cmd.AddParameter("name", name);
		return (await cmd.ExecuteSelect<Brand>(Map))!;
	}

	public async Task<Brand?> UpdateAsync(int id, string name) {
		
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "UPDATE brands SET name = @name WHERE id = @id RETURNING id, name";
		cmd.AddParameter("id", id);
		cmd.AddParameter("name", name);
		return await cmd.ExecuteSelect<Brand>(Map);
	}

	public async Task<bool> DeleteAsync(int id) {
		
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "DELETE FROM brands WHERE id = @id";
		cmd.AddParameter("id", id);
		return await cmd.ExecuteCommandNonQuery();
	}
}
