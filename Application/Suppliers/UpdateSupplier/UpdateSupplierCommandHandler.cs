using API.Application.Suppliers;
using API.Domain.Suppliers;
using API.Persistence.Suppliers.Interfaces;
using MediatR;

namespace API.Application.Suppliers.UpdateSupplier;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, SupplierDto?>
{
    private readonly ISupplierRepository _supplierRepository;

    public UpdateSupplierCommandHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDto?> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.UpdateAsync(request.Id, request.Name, request.Phone, request.Email, request.Notes);
        return supplier == null ? null : SupplierDto.From(supplier);
    }
}
