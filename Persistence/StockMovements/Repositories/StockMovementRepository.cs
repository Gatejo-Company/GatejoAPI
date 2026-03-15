using API.DataAccess.Interfaces;
using API.Domain.StockMovements;
using API.Persistence.Shared;
using API.Persistence.StockMovements.Interfaces;
using API.Utils.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace API.Persistence.StockMovements.Repositories;

[Injectable(ServiceLifetime.Scoped)]
public class StockMovementRepository : IStockMovementRepository {
    private readonly ICConnection _connection;

    public StockMovementRepository(ICConnection connection) {
        _connection = connection;
    }

    private static void Map(StockMovement obj, ICDataReader rs) {
        obj.Id = rs.GetValue<int>("id");
        obj.ProductId = rs.GetValue<int>("product_id");
        obj.ProductName = rs.GetValue<string>("product_name");
        obj.TypeId = rs.GetValue<int>("type_id");
        obj.TypeName = rs.GetValue<string>("type_name");
        obj.Quantity = rs.GetValue<int>("quantity");
        obj.ReferenceId = rs.GetValue<int?>("reference_id");
        obj.Notes = rs.GetValue<string?>("notes");
        obj.CreatedAt = rs.GetValue<DateTime>("created_at");
    }

    private const string SelectSql = @"
        SELECT sm.id, sm.product_id, p.name AS product_name,
               sm.type_id, mt.name AS type_name,
               sm.quantity, sm.reference_id, sm.notes, sm.created_at
        FROM stock_movements sm
        INNER JOIN products p ON p.id = sm.product_id
        INNER JOIN movement_types mt ON mt.id = sm.type_id";

    private static (string where, List<Action<ICCommand>> applyParams) BuildWhere(StockMovementsFilter filter) {
        var conditions = new List<string>();
        var actions = new List<Action<ICCommand>>();

        if (filter.ProductId.HasValue) {
            conditions.Add("sm.product_id = @pid");
            actions.Add(cmd => cmd.AddParameter("pid", filter.ProductId.Value));
        }
        if (filter.TypeId.HasValue) {
            conditions.Add("sm.type_id = @tid");
            actions.Add(cmd => cmd.AddParameter("tid", filter.TypeId.Value));
        }
        if (filter.From.HasValue) {
            conditions.Add("sm.created_at >= @from");
            actions.Add(cmd => cmd.AddParameter("from", filter.From.Value));
        }
        if (filter.To.HasValue) {
            conditions.Add("sm.created_at <= @to");
            actions.Add(cmd => cmd.AddParameter("to", filter.To.Value));
        }

        var where = conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "";
        return (where, actions);
    }

    public async Task<PagedData<StockMovement>> GetAllAsync(StockMovementsFilter filter) {
        await _connection.Connect();
        var (where, applyParams) = BuildWhere(filter);
        return await PaginationHelper.FetchPagedAsync<StockMovement>(
            _connection,
            $@"SELECT sm.id, sm.product_id, p.name AS product_name,
			          sm.type_id, mt.name AS type_name,
			          sm.quantity, sm.reference_id, sm.notes, sm.created_at,
			          COUNT(*) OVER() AS total_count
			   FROM stock_movements sm
			   INNER JOIN products p ON p.id = sm.product_id
			   INNER JOIN movement_types mt ON mt.id = sm.type_id {where} ORDER BY sm.created_at DESC",
            cmd => { foreach (var apply in applyParams) apply(cmd); },
            Map, filter.Page, filter.PageSize);
    }

    public async Task<StockMovement?> GetByIdAsync(int id) {
        await _connection.Connect();
        var cmd = _connection.CreateCommand();
        cmd.CommandText = SelectSql + " WHERE sm.id = @id";
        cmd.AddParameter("id", id);
        return await cmd.ExecuteSelect<StockMovement>(Map);
    }

    public async Task<StockMovement> CreateManualAdjustmentAsync(int productId, int typeId, int quantity, string? notes) {
        await _connection.Connect();

        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            WITH ins AS (
                INSERT INTO stock_movements (product_id, type_id, quantity, notes)
                VALUES (@pid, @tid, @qty, @notes)
                RETURNING *
            )
            SELECT sm.id, sm.product_id, p.name AS product_name,
                   sm.type_id, mt.name AS type_name,
                   sm.quantity, sm.reference_id, sm.notes, sm.created_at
            FROM ins sm
            LEFT JOIN products p ON p.id = sm.product_id
            LEFT JOIN movement_types mt ON mt.id = sm.type_id
            WHERE sm.id = (SELECT id FROM ins)";
        cmd.AddParameter("pid", productId);
        cmd.AddParameter("tid", typeId);
        cmd.AddParameter("qty", quantity);
        cmd.AddParameter("notes", (object?)notes ?? DBNull.Value);
        return (await cmd.ExecuteSelect<StockMovement>(Map))!;
    }

    public async Task<List<StockMovement>> GetByProductAsync(int productId) {
        await _connection.Connect();
        var cmd = _connection.CreateCommand();
        cmd.CommandText = SelectSql + " WHERE sm.product_id = @pid ORDER BY sm.created_at DESC";
        cmd.AddParameter("pid", productId);
        return await cmd.ExecuteSelectList<StockMovement>(Map);
    }

    public async Task CreateForInvoiceAsync(string movementType, int productId, int quantity, int referenceId, string notes) {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO stock_movements (product_id, type_id, quantity, reference_id, notes)
            VALUES (@pid, (SELECT id FROM movement_types WHERE name = @type LIMIT 1),
                    @qty, @refId, @notes)";
        cmd.AddParameter("pid", productId);
        cmd.AddParameter("type", movementType);
        cmd.AddParameter("qty", quantity);
        cmd.AddParameter("refId", referenceId);
        cmd.AddParameter("notes", notes);
        await cmd.ExecuteCommandNonQuery();
    }

    public async Task CreateReversalAsync(int productId, int referenceId, int quantityDelta, string notes) {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO stock_movements (product_id, type_id, quantity, reference_id, notes)
            VALUES (@pid, (SELECT id FROM movement_types WHERE name = @adjType LIMIT 1),
                    @qty, @refId, @notes)";
        cmd.AddParameter("pid", productId);
        cmd.AddParameter("adjType", MovementTypeNames.StockAdjustment);
        cmd.AddParameter("qty", quantityDelta);
        cmd.AddParameter("refId", referenceId);
        cmd.AddParameter("notes", notes);
        await cmd.ExecuteCommandNonQuery();
    }
}
