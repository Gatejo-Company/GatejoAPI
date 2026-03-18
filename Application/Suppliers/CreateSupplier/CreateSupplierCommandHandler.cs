using API.Application.Suppliers;
using API.Domain.Suppliers;
using API.Persistence.Suppliers.Interfaces;
using MediatR;

namespace API.Application.Suppliers.CreateSupplier;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, SupplierDto>
{
    private readonly ISupplierRepository _supplierRepository;

    public CreateSupplierCommandHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDto> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.CreateAsync(request.Name, request.Phone, request.Email, request.Notes);
        return SupplierDto.From(supplier);
    }
}
