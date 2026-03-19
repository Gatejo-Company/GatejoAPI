using API.Application.SaleInvoices.GetSalesSummary12Months;
using API.DataAccess.Interfaces;
using API.Domain.SaleInvoices;
using API.Persistence.SaleInvoices.Interfaces;
using API.Persistence.Shared;
using API.Utils.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace API.Persistence.SaleInvoices.Repositories;

[Injectable(ServiceLifetime.Scoped)]
public class SaleInvoiceRepository : ISaleInvoiceRepository {
    private readonly ICConnection _connection;

    public SaleInvoiceRepository(ICConnection connection) {
        _connection = connection;
    }

    private static void MapInvoice(SaleInvoice obj, ICDataReader rs) {
        obj.Id = rs.GetValue<int>("id");
        obj.Date = rs.GetValue<DateOnly>("date");
        obj.Total = rs.GetValue<decimal>("total");
        obj.OnCredit = rs.GetValue<bool>("on_credit");
        obj.Reversed = rs.GetValue<bool>("reversed");
        obj.PaidAt = rs.GetValue<DateTime?>("paid_at");
        obj.Notes = rs.GetValue<string?>("notes");
        obj.CreatedAt = rs.GetValue<DateTime>("created_at");
    }

    private static void MapItem(SaleItem obj, ICDataReader rs) {
        obj.Id = rs.GetValue<int>("id");
        obj.SaleInvoiceId = rs.GetValue<int>("sale_invoice_id");
        obj.ProductId = rs.GetValue<int>("product_id");
        obj.ProductName = rs.GetValue<string>("product_name");
        obj.Quantity = rs.GetValue<int>("quantity");
        obj.UnitPrice = rs.GetValue<decimal>("unit_price");
        obj.Subtotal = rs.GetValue<decimal>("subtotal");
    }

    private static string BuildWhere(SaleInvoicesFilter filter, ICCommand cmd) {
        var conditions = new List<string>();

        if (filter.From.HasValue) { conditions.Add("si.date >= @from"); cmd.AddParameter("from", filter.From.Value); }
        if (filter.To.HasValue) { conditions.Add("si.date <= @to"); cmd.AddParameter("to", filter.To.Value); }
        if (filter.OnCredit.HasValue) { conditions.Add("si.on_credit = @onCredit"); cmd.AddParameter("onCredit", filter.OnCredit.Value); }
        if (filter.Paid == true) conditions.Add("si.paid_at IS NOT NULL");
        if (filter.Paid == false) conditions.Add("si.paid_at IS NULL");

        return conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "";
    }

    public async Task<PagedData<SaleInvoice>> GetAllAsync(SaleInvoicesFilter filter) {
        var (items, totalCount) = await FetchPageWithItems(filter, filter.Page, filter.PageSize);

        if (items.Count == 0 && filter.Page > 1) {
            (items, totalCount) = await FetchPageWithItems(filter, 1, filter.PageSize);
            return new PagedData<SaleInvoice>(items, totalCount, 1);
        }

        return new PagedData<SaleInvoice>(items, totalCount, filter.Page);
    }

    private async Task<(List<SaleInvoice>, int)> FetchPageWithItems(SaleInvoicesFilter filter, int page, int pageSize) {
        var cmd = _connection.CreateCommand();
        var where = BuildWhere(filter, cmd);
        cmd.CommandText = $@"
			WITH invoices AS (
				SELECT si.id, si.date, si.total, si.on_credit, si.reversed, si.paid_at, si.notes, si.created_at,
				       COUNT(*) OVER() AS total_count
				FROM sale_invoices si{where}
				ORDER BY si.date DESC, si.id DESC
				LIMIT @pageSize OFFSET @offset
			)
			SELECT inv.id, inv.date, inv.total, inv.on_credit, inv.reversed, inv.paid_at, inv.notes, inv.created_at, inv.total_count,
			       it.id AS item_id, it.sale_invoice_id, it.product_id, pr.name AS product_name,
			       it.quantity, it.unit_price, it.subtotal
			FROM invoices inv
			LEFT JOIN sale_items it ON it.sale_invoice_id = inv.id
			LEFT JOIN products pr ON pr.id = it.product_id
			ORDER BY inv.date DESC, inv.id DESC";
        cmd.AddParameter("pageSize", pageSize);
        cmd.AddParameter("offset", (page - 1) * pageSize);

        var items = new List<SaleInvoice>();
        var totalCount = 0;

        await cmd.ExecuteCommandQuery(rs => {
            totalCount = rs.GetValue<int>("total_count");

            if (items.Count == 0 || items[^1].Id != rs.GetValue<int>("id")) {
                var invoice = new SaleInvoice();
                MapInvoice(invoice, rs);
                items.Add(invoice);
            }

            var itemId = rs.GetValue<int?>("item_id");
            if (itemId.HasValue) {
                items[^1].Items.Add(new SaleItem {
                    Id = itemId.Value,
                    SaleInvoiceId = rs.GetValue<int>("sale_invoice_id"),
                    ProductId = rs.GetValue<int>("product_id"),
                    ProductName = rs.GetValue<string>("product_name"),
                    Quantity = rs.GetValue<int>("quantity"),
                    UnitPrice = rs.GetValue<decimal>("unit_price"),
                    Subtotal = rs.GetValue<decimal>("subtotal")
                });
            }
        });

        return (items, totalCount);
    }

    public async Task<SaleInvoice?> GetByIdAsync(int id) {

        var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT id, date, total, on_credit, reversed, paid_at, notes, created_at FROM sale_invoices WHERE id = @id";
        cmd.AddParameter("id", id);
        var invoice = await cmd.ExecuteSelect<SaleInvoice>(MapInvoice);
        if (invoice == null) return null;

        var itemCmd = _connection.CreateCommand();
        itemCmd.CommandText = @"
            SELECT si.id, si.sale_invoice_id, si.product_id, p.name AS product_name,
                   si.quantity, si.unit_price, si.subtotal
            FROM sale_items si
            INNER JOIN products p ON p.id = si.product_id
            WHERE si.sale_invoice_id = @id";
        itemCmd.AddParameter("id", id);
        invoice.Items = await itemCmd.ExecuteSelectList<SaleItem>(MapItem);

        return invoice;
    }

    public async Task<int> CreateAsync(DateOnly date, bool onCredit, bool reversed, decimal total, string? notes) {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO sale_invoices (date, total, on_credit, reversed, notes)
            VALUES (@date, @total, @onCredit, @reversed, @notes)
            RETURNING id";
        cmd.AddParameter("date", date);
        cmd.AddParameter("total", total);
        cmd.AddParameter("onCredit", onCredit);
        cmd.AddParameter("reversed", reversed);
        cmd.AddParameter("notes", (object?)notes ?? DBNull.Value);
        return await cmd.ExecuteGetValue<int>("id");
    }

    public async Task AddItemAsync(int invoiceId, int productId, int quantity, decimal unitPrice, decimal subtotal) {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO sale_items (sale_invoice_id, product_id, quantity, unit_price, subtotal)
            VALUES (@invoiceId, @pid, @qty, @price, @subtotal)";
        cmd.AddParameter("invoiceId", invoiceId);
        cmd.AddParameter("pid", productId);
        cmd.AddParameter("qty", quantity);
        cmd.AddParameter("price", unitPrice);
        cmd.AddParameter("subtotal", subtotal);
        await cmd.ExecuteCommandNonQuery();
    }

    public async Task<bool> MarkAsPaidAsync(int id) {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = "UPDATE sale_invoices SET paid_at = NOW() WHERE id = @id AND on_credit = TRUE AND paid_at IS NULL";
        cmd.AddParameter("id", id);
        return await cmd.ExecuteCommandNonQuery();
    }

    public async Task<bool> ReverseAsync(int id) {

        var cmd = _connection.CreateCommand();
        cmd.CommandText = "UPDATE sale_invoices SET reversed = TRUE WHERE id = @id AND reversed = FALSE";
        cmd.AddParameter("id", id);
        return await cmd.ExecuteCommandNonQuery();
    }

    public async Task<bool> DeleteAsync(int id) {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM sale_invoices WHERE id = @id";
        cmd.AddParameter("id", id);
        return await cmd.ExecuteCommandNonQuery();
    }

    public async Task<PagedData<SaleInvoice>> GetPendingCreditAsync(PagedFilter filter) {
        return await PaginationHelper.FetchPagedAsync<SaleInvoice>(
            _connection,
            @"SELECT id, date, total, on_credit, reversed, paid_at, notes, created_at, COUNT(*) OVER() AS total_count
			  FROM sale_invoices
			  WHERE on_credit = TRUE AND paid_at IS NULL
			  ORDER BY date ASC",
            null, MapInvoice, filter.Page, filter.PageSize);
    }

    public async Task<List<MonthlySalesSummaryDto>> GetSalesSummaryLast12MonthsAsync() {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            WITH months AS (
                SELECT generate_series(
                    date_trunc('month', NOW() - INTERVAL '11 months'),
                    date_trunc('month', NOW()),
                    INTERVAL '1 month'
                )::date AS month_start
            )
            SELECT
                EXTRACT(YEAR FROM m.month_start)::int AS year,
                EXTRACT(MONTH FROM m.month_start)::int AS month,
                COUNT(si.id)::int AS invoice_count,
                COALESCE(SUM(si.total), 0)::numeric AS total_amount
            FROM months m
            LEFT JOIN sale_invoices si
                ON date_trunc('month', si.date::timestamp) = m.month_start::timestamp
                AND si.reversed = FALSE
            GROUP BY m.month_start
            ORDER BY m.month_start";

        var result = new List<MonthlySalesSummaryDto>();

        await cmd.ExecuteCommandQuery(rs => {
            result.Add(new MonthlySalesSummaryDto(
                rs.GetValue<int>("year"),
                rs.GetValue<int>("month"),
                rs.GetValue<int>("invoice_count"),
                rs.GetValue<decimal>("total_amount")
            ));
        });

        return result;
    }
}
