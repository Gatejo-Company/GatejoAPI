using API.Application.Shared;
using API.Application.Suppliers;
using MediatR;

namespace API.Application.Suppliers.GetSuppliers;

public record GetSuppliersQuery(int Page, int PageSize) : IRequest<PagedResult<SupplierDto>>, IPagedQuery;
