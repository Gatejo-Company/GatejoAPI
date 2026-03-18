using MediatR;

namespace API.Application.Suppliers.DeleteSupplier;

public record DeleteSupplierCommand(int Id) : IRequest<bool>;
