using API.Application.Shared;
using API.Application.Suppliers;
using API.Domain.Suppliers;
using API.Persistence.Shared;
using API.Persistence.Suppliers.Interfaces;
using MediatR;

namespace API.Application.Suppliers.GetSuppliers;

public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, PagedResult<SupplierDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSuppliersQueryHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<PagedResult<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var filter = new PagedFilter(request.Page, request.PageSize);
        var data = await _supplierRepository.GetAllAsync(filter);
        return data.ToPagedResult(filter.PageSize, SupplierDto.From);
    }
}
