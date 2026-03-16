using API.DataAccess.Interfaces;
using API.Domain.Suppliers;
using API.Persistence.Shared;
using API.Persistence.Suppliers.Interfaces;
using API.Utils.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace API.Persistence.Suppliers.Repositories;

[Injectable(ServiceLifetime.Scoped)]
public class SupplierRepository : ISupplierRepository {
	private readonly ICConnection _connection;

	public SupplierRepository(ICConnection connection) {
		_connection = connection;
	}

	private static void Map(Supplier obj, ICDataReader rs) {
		obj.Id = rs.GetValue<int>("id");
		obj.Name = rs.GetValue<string>("name");
		obj.Phone = rs.GetValue<string?>("phone");
		obj.Email = rs.GetValue<string?>("email");
		obj.Notes = rs.GetValue<string?>("notes");
	}

	public async Task<PagedData<Supplier>> GetAllAsync(PagedFilter filter) {
		
		return await PaginationHelper.FetchPagedAsync<Supplier>(
			_connection,
			"SELECT id, name, phone, email, notes, COUNT(*) OVER() AS total_count FROM suppliers ORDER BY id",
			null, Map, filter.Page, filter.PageSize);
	}

	public async Task<Supplier?> GetByIdAsync(int id) {
		
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "SELECT id, name, phone, email, notes FROM suppliers WHERE id = @id";
		cmd.AddParameter("id", id);
		return await cmd.ExecuteSelect<Supplier>(Map);
	}

	public async Task<Supplier> CreateAsync(string name, string? phone, string? email, string? notes) {
		
		var cmd = _connection.CreateCommand();
		cmd.CommandText = @"
            INSERT INTO suppliers (name, phone, email, notes)
            VALUES (@name, @phone, @email, @notes)
            RETURNING id, name, phone, email, notes";
		cmd.AddParameter("name", name);
		cmd.AddParameter("phone", (object?)phone ?? DBNull.Value);
		cmd.AddParameter("email", (object?)email ?? DBNull.Value);
		cmd.AddParameter("notes", (object?)notes ?? DBNull.Value);
		return (await cmd.ExecuteSelect<Supplier>(Map))!;
	}

	public async Task<Supplier?> UpdateAsync(int id, string name, string? phone, string? email, string? notes) {
		
		var cmd = _connection.CreateCommand();
		cmd.CommandText = @"
            UPDATE suppliers SET name = @name, phone = @phone, email = @email, notes = @notes
            WHERE id = @id
            RETURNING id, name, phone, email, notes";
		cmd.AddParameter("id", id);
		cmd.AddParameter("name", name);
		cmd.AddParameter("phone", (object?)phone ?? DBNull.Value);
		cmd.AddParameter("email", (object?)email ?? DBNull.Value);
		cmd.AddParameter("notes", (object?)notes ?? DBNull.Value);
		return await cmd.ExecuteSelect<Supplier>(Map);
	}

	public async Task<bool> DeleteAsync(int id) {
		
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "DELETE FROM suppliers WHERE id = @id";
		cmd.AddParameter("id", id);
		return await cmd.ExecuteCommandNonQuery();
	}
}
