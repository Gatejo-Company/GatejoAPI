using API.Application.Brands;
using API.Application.Shared;
using MediatR;

namespace API.Application.Brands.GetBrands;

public record GetBrandsQuery(int Page, int PageSize) : IRequest<PagedResult<BrandDto>>, IPagedQuery;
