using API.DataAccess.Interfaces;
using API.Domain.PurchaseInvoices;
using API.Persistence.PurchaseInvoices.Interfaces;
using API.Persistence.Shared;
using API.Utils.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace API.Persistence.PurchaseInvoices.Repositories;

[Injectable(ServiceLifetime.Scoped)]
public class PurchaseInvoiceRepository : IPurchaseInvoiceRepository {
	private readonly ICConnection _connection;

	public PurchaseInvoiceRepository(ICConnection connection) {
		_connection = connection;
	}

	private static void MapInvoice(PurchaseInvoice obj, ICDataReader rs) {
		obj.Id = rs.GetValue<int>("id");
		obj.SupplierId = rs.GetValue<int>("supplier_id");
		obj.SupplierName = rs.GetValue<string>("supplier_name");
		obj.Date = rs.GetValue<DateOnly>("date");
		obj.Total = rs.GetValue<decimal>("total");
		obj.Paid = rs.GetValue<decimal>("paid");
		obj.Notes = rs.GetValue<string?>("notes");
		obj.CreatedAt = rs.GetValue<DateTime>("created_at");
	}

	private static void MapItem(PurchaseItem obj, ICDataReader rs) {
		obj.Id = rs.GetValue<int>("id");
		obj.PurchaseInvoiceId = rs.GetValue<int>("purchase_invoice_id");
		obj.ProductId = rs.GetValue<int>("product_id");
		obj.ProductName = rs.GetValue<string>("product_name");
		obj.Quantity = rs.GetValue<int>("quantity");
		obj.UnitPrice = rs.GetValue<decimal>("unit_price");
	}

	private static (string where, List<Action<ICCommand>> applyParams) BuildWhere(PurchaseInvoicesFilter filter) {
		var conditions = new List<string>();
		var actions = new List<Action<ICCommand>>();

		if (filter.SupplierId.HasValue) {
			conditions.Add("pi.supplier_id = @sid");
			actions.Add(cmd => cmd.AddParameter("sid", filter.SupplierId.Value));
		}
		if (filter.From.HasValue) {
			conditions.Add("pi.date >= @from");
			actions.Add(cmd => cmd.AddParameter("from", filter.From.Value));
		}
		if (filter.To.HasValue) {
			conditions.Add("pi.date <= @to");
			actions.Add(cmd => cmd.AddParameter("to", filter.To.Value));
		}

		var where = conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "";
		return (where, actions);
	}

	public async Task<PagedData<PurchaseInvoice>> GetAllAsync(PurchaseInvoicesFilter filter) {
		await _connection.Connect();
		var (where, applyParams) = BuildWhere(filter);
		return await PaginationHelper.FetchPagedAsync<PurchaseInvoice>(
			_connection,
			$@"SELECT pi.id, pi.supplier_id, s.name AS supplier_name, pi.date, pi.total, pi.paid, pi.notes, pi.created_at,
			          COUNT(*) OVER() AS total_count
			   FROM purchase_invoices pi
			   INNER JOIN suppliers s ON s.id = pi.supplier_id{where} ORDER BY pi.date DESC, pi.id DESC",
			cmd => { foreach (var apply in applyParams) apply(cmd); },
			MapInvoice, filter.Page, filter.PageSize);
	}

	public async Task<PurchaseInvoice?> GetByIdAsync(int id) {
		await _connection.Connect();

		var cmd = _connection.CreateCommand();
		cmd.CommandText = @"
            SELECT pi.id, pi.supplier_id, s.name AS supplier_name, pi.date, pi.total, pi.paid, pi.notes, pi.created_at
            FROM purchase_invoices pi
            INNER JOIN suppliers s ON s.id = pi.supplier_id
            WHERE pi.id = @id";
		cmd.AddParameter("id", id);
		var invoice = await cmd.ExecuteSelect<PurchaseInvoice>(MapInvoice);
		if (invoice == null) return null;

		var itemCmd = _connection.CreateCommand();
		itemCmd.CommandText = @"
            SELECT pitem.id, pitem.purchase_invoice_id, pitem.product_id, p.name AS product_name,
                   pitem.quantity, pitem.unit_price
            FROM purchase_items pitem
            INNER JOIN products p ON p.id = pitem.product_id
            WHERE pitem.purchase_invoice_id = @id";
		itemCmd.AddParameter("id", id);
		invoice.Items = await itemCmd.ExecuteSelectList<PurchaseItem>(MapItem);

		return invoice;
	}

	public async Task<int> CreateAsync(int supplierId, DateOnly date, decimal total, string? notes) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = @"
            INSERT INTO purchase_invoices (supplier_id, date, total, notes)
            VALUES (@sid, @date, @total, @notes)
            RETURNING id";
		cmd.AddParameter("sid", supplierId);
		cmd.AddParameter("date", date);
		cmd.AddParameter("total", total);
		cmd.AddParameter("notes", (object?)notes ?? DBNull.Value);
		return await cmd.ExecuteGetValue<int>("id");
	}

	public async Task AddItemAsync(int invoiceId, int productId, int quantity, decimal unitPrice) {
		var cmd = _connection.CreateCommand();
		cmd.CommandText = @"
            INSERT INTO purchase_items (purchase_invoice_id, product_id, quantity, unit_price)
            VALUES (@invoiceId, @pid, @qty, @price)";
		cmd.AddParameter("invoiceId", invoiceId);
		cmd.AddParameter("pid", productId);
		cmd.AddParameter("qty", quantity);
		cmd.AddParameter("price", unitPrice);
		await cmd.ExecuteCommandNonQuery();
	}

	public async Task<bool> UpdatePaymentAsync(int id, decimal paid) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "UPDATE purchase_invoices SET paid = @paid WHERE id = @id";
		cmd.AddParameter("paid", paid);
		cmd.AddParameter("id", id);
		return await cmd.ExecuteCommandNonQuery();
	}

	public async Task<bool> DeleteAsync(int id) {
		await _connection.Connect();
		var cmd = _connection.CreateCommand();
		cmd.CommandText = "DELETE FROM purchase_invoices WHERE id = @id";
		cmd.AddParameter("id", id);
		return await cmd.ExecuteCommandNonQuery();
	}
}
