using API.Application.Suppliers;
using MediatR;

namespace API.Application.Suppliers.CreateSupplier;

public record CreateSupplierCommand(string Name, string? Phone, string? Email, string? Notes) : IRequest<SupplierDto>;
