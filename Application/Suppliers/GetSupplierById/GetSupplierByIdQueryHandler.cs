using API.Application.Suppliers;
using API.Domain.Suppliers;
using API.Persistence.Suppliers.Interfaces;
using MediatR;

namespace API.Application.Suppliers.GetSupplierById;

public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDto?>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSupplierByIdQueryHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDto?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id);
        return supplier == null ? null : SupplierDto.From(supplier);
    }
}
