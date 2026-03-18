using API.Application.Suppliers;
using MediatR;

namespace API.Application.Suppliers.GetSupplierById;

public record GetSupplierByIdQuery(int Id) : IRequest<SupplierDto?>;
