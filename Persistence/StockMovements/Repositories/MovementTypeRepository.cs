using API.DataAccess.Interfaces;
using API.Domain.StockMovements;
using API.Persistence.Shared;
using API.Persistence.StockMovements.Interfaces;
using API.Utils.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace API.Persistence.StockMovements.Repositories;

[Injectable(ServiceLifetime.Scoped)]
public class MovementTypeRepository : IMovementTypeRepository {
	private readonly ICConnection _connection;

	public MovementTypeRepository(ICConnection connection) {
		_connection = connection;
	}

	private static void Map(MovementType obj, ICDataReader rs) {
		obj.Id = rs.GetValue<int>("id");
		obj.Name = rs.GetValue<string>("name");
	}

	public async Task<PagedData<MovementType>> GetAllAsync(PagedFilter filter) {
		await _connection.Connect();
		return await PaginationHelper.FetchPagedAsync<MovementType>(
			_connection,
			"SELECT id, name, COUNT(*) OVER() AS total_count FROM movement_types ORDER BY id",
			null, Map, filter.Page, filter.PageSize);
	}

	public async Task<MovementType?> GetByIdAsync(int id) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "SELECT id, name FROM movement_types WHERE id = @id";
		cmd.AddParameter("id", id);
		return await cmd.ExecuteSelect<MovementType>(Map);
	}
}
