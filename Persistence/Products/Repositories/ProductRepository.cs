using API.DataAccess.Interfaces;
using API.Domain.Products;
using API.Persistence.Products.Interfaces;
using API.Persistence.Shared;
using API.Utils.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace API.Persistence.Products.Repositories;

[Injectable(ServiceLifetime.Scoped)]
public class ProductRepository : IProductRepository {
    private readonly ICConnection _connection;

    public ProductRepository(ICConnection connection) {
        _connection = connection;
    }

    private static void MapProduct(Product obj, ICDataReader rs) {
        obj.Id = rs.GetValue<int>("id");
        obj.Name = rs.GetValue<string>("name");
        obj.Description = rs.GetValue<string?>("description");
        obj.CategoryId = rs.GetValue<int>("category_id");
        obj.CategoryName = rs.GetValue<string>("category_name");
        obj.BrandId = rs.GetValue<int>("brand_id");
        obj.BrandName = rs.GetValue<string>("brand_name");
        obj.Price = rs.GetValue<decimal>("price");
        obj.MinStock = rs.GetValue<int>("min_stock");
        obj.Active = rs.GetValue<bool>("active");
        obj.CreatedAt = rs.GetValue<DateTime>("created_at");
        obj.CurrentStock = rs.GetValue<int>("current_stock");
    }

    private const string SelectSql = @"
        SELECT p.id, p.name, p.description, p.category_id, c.name AS category_name,
               p.brand_id, b.name AS brand_name, p.price, p.min_stock, p.active, p.created_at,
               COALESCE((SELECT SUM(quantity) FROM stock_movements WHERE product_id = p.id), 0) AS current_stock
        FROM products p
        INNER JOIN categories c ON c.id = p.category_id
        INNER JOIN brands b ON b.id = p.brand_id";

    private static (string where, List<Action<ICCommand>> applyParams) BuildWhere(ProductsFilter filter) {
        var conditions = new List<string>();
        var actions = new List<Action<ICCommand>>();

        if (filter.CategoryId.HasValue) {
            conditions.Add("p.category_id = @catId");
            actions.Add(cmd => cmd.AddParameter("catId", filter.CategoryId.Value));
        }
        if (filter.BrandId.HasValue) {
            conditions.Add("p.brand_id = @brandId");
            actions.Add(cmd => cmd.AddParameter("brandId", filter.BrandId.Value));
        }
        if (filter.Active.HasValue) {
            conditions.Add("p.active = @active");
            actions.Add(cmd => cmd.AddParameter("active", filter.Active.Value));
        }
        if (filter.LowStock == true) {
            conditions.Add("COALESCE((SELECT SUM(quantity) FROM stock_movements WHERE product_id = p.id), 0) < p.min_stock");
        }

        var where = conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "";
        return (where, actions);
    }

    public async Task<PagedData<Product>> GetAllAsync(ProductsFilter filter) {
        
        var (where, applyParams) = BuildWhere(filter);
        var sql = @"
        SELECT p.id, p.name, p.description, p.category_id, c.name AS category_name,
               p.brand_id, b.name AS brand_name, p.price, p.min_stock, p.active, p.created_at,
               COALESCE((SELECT SUM(quantity) FROM stock_movements WHERE product_id = p.id), 0) AS current_stock,
               COUNT(*) OVER() AS total_count
        FROM products p
        INNER JOIN categories c ON c.id = p.category_id
        INNER JOIN brands b ON b.id = p.brand_id"
            + where + " ORDER BY p.id";
        return await PaginationHelper.FetchPagedAsync<Product>(
            _connection, sql,
            cmd => { foreach (var apply in applyParams) apply(cmd); },
            MapProduct, filter.Page, filter.PageSize);
    }

    public async Task<Product?> GetByIdAsync(int id) {
        
        var cmd = _connection.CreateCommand();
        cmd.CommandText = SelectSql + " WHERE p.id = @id";
        cmd.AddParameter("id", id);
        return await cmd.ExecuteSelect<Product>(MapProduct);
    }

    public async Task<Product> CreateAsync(string name, string? description, int categoryId, int brandId, decimal price, int minStock) {
        
        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            WITH ins AS (
                INSERT INTO products (name, description, category_id, brand_id, price, min_stock)
                VALUES (@name, @description, @categoryId, @brandId, @price, @minStock)
                RETURNING *
            ),
            ph AS (
                INSERT INTO price_history (product_id, price, reason)
                SELECT id, @price, 'Initial price' FROM ins
            )
            SELECT p.id, p.name, p.description, p.category_id, c.name AS category_name,
                   p.brand_id, b.name AS brand_name, p.price, p.min_stock, p.active, p.created_at,
                   0 AS current_stock
            FROM ins p
            LEFT JOIN categories c ON c.id = p.category_id
            LEFT JOIN brands b ON b.id = p.brand_id
            WHERE p.id = (SELECT id FROM ins)";
        cmd.AddParameter("name", name);
        cmd.AddParameter("description", (object?)description ?? DBNull.Value);
        cmd.AddParameter("categoryId", categoryId);
        cmd.AddParameter("brandId", brandId);
        cmd.AddParameter("price", price);
        cmd.AddParameter("minStock", minStock);
        return (await cmd.ExecuteSelect<Product>(MapProduct))!;
    }

    public async Task<Product?> UpdateAsync(int id, string name, string? description, int categoryId, int brandId, int minStock) {
        
        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            WITH upd AS (
                UPDATE products SET name = @name, description = @description,
                    category_id = @categoryId, brand_id = @brandId, min_stock = @minStock
                WHERE id = @id
                RETURNING *
            )
            SELECT p.id, p.name, p.description, p.category_id, c.name AS category_name,
                   p.brand_id, b.name AS brand_name, p.price, p.min_stock, p.active, p.created_at,
                   COALESCE((SELECT SUM(quantity) FROM stock_movements WHERE product_id = p.id), 0) AS current_stock
            FROM upd p
            LEFT JOIN categories c ON c.id = p.category_id
            LEFT JOIN brands b ON b.id = p.brand_id
            WHERE p.id = (SELECT id FROM upd)";
        cmd.AddParameter("id", id);
        cmd.AddParameter("name", name);
        cmd.AddParameter("description", (object?)description ?? DBNull.Value);
        cmd.AddParameter("categoryId", categoryId);
        cmd.AddParameter("brandId", brandId);
        cmd.AddParameter("minStock", minStock);
        return await cmd.ExecuteSelect<Product>(MapProduct);
    }

    public async Task<Product?> UpdatePriceAsync(int id, decimal price, string? reason) {
        
        var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            WITH upd AS (
                UPDATE products SET price = @price
                WHERE id = @id
                RETURNING id
            ),
            ph AS (
                INSERT INTO price_history (product_id, price, reason)
                SELECT id, @price, @reason FROM upd
            )
            SELECT p.id, p.name, p.description, p.category_id, c.name AS category_name,
                   p.brand_id, b.name AS brand_name, p.price, p.min_stock, p.active, p.created_at,
                   COALESCE((SELECT SUM(quantity) FROM stock_movements WHERE product_id = p.id), 0) AS current_stock
            FROM products p
            LEFT JOIN categories c ON c.id = p.category_id
            LEFT JOIN brands b ON b.id = p.brand_id
            WHERE p.id = (SELECT id FROM upd)";
        cmd.AddParameter("id", id);
        cmd.AddParameter("price", price);
        cmd.AddParameter("reason", (object?)reason ?? DBNull.Value);
        return await cmd.ExecuteSelect<Product>(MapProduct);
    }

    public async Task<bool> SetActiveAsync(int id, bool active) {
        
        var cmd = _connection.CreateCommand();
        cmd.CommandText = "UPDATE products SET active = @active WHERE id = @id";
        cmd.AddParameter("id", id);
        cmd.AddParameter("active", active);
        return await cmd.ExecuteCommandNonQuery();
    }

    public async Task<bool> DeleteAsync(int id) {
        
        var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM products WHERE id = @id";
        cmd.AddParameter("id", id);
        return await cmd.ExecuteCommandNonQuery();
    }

    public async Task<PagedData<PriceHistory>> GetPriceHistoryAsync(PriceHistoryFilter filter) {
        
        return await PaginationHelper.FetchPagedAsync<PriceHistory>(
            _connection,
            @"SELECT id, product_id, price, reason, created_at, COUNT(*) OVER() AS total_count
			  FROM price_history
			  WHERE product_id = @pid
			  ORDER BY created_at DESC",
            cmd => cmd.AddParameter("pid", filter.ProductId),
            (obj, rs) => {
                obj.Id = rs.GetValue<int>("id");
                obj.ProductId = rs.GetValue<int>("product_id");
                obj.Price = rs.GetValue<decimal>("price");
                obj.Reason = rs.GetValue<string?>("reason");
                obj.CreatedAt = rs.GetValue<DateTime>("created_at");
            },
            filter.Page, filter.PageSize);
    }

    public async Task<int> GetCurrentStockAsync(int productId) {
        
        var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT COALESCE(SUM(quantity), 0) AS stock FROM stock_movements WHERE product_id = @pid";
        cmd.AddParameter("pid", productId);
        return await cmd.ExecuteGetValue<int>("stock");
    }
}
