using API.Domain.Suppliers;

namespace API.Application.Suppliers;

public record SupplierDto(int Id, string Name, string? Phone, string? Email, string? Notes) {
	public static SupplierDto From(Supplier supplier) => new(supplier.Id, supplier.Name, supplier.Phone, supplier.Email, supplier.Notes);
}
