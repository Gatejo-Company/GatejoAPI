using API.Application.Suppliers;
using MediatR;

namespace API.Application.Suppliers.UpdateSupplier;

public record UpdateSupplierCommand(int Id, string Name, string? Phone, string? Email, string? Notes) : IRequest<SupplierDto?>;
