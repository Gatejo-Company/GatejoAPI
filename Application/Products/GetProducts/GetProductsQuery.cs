using API.Application.Products;
using API.Application.Shared;
using MediatR;

namespace API.Application.Products.GetProducts;

public record GetProductsQuery(int? CategoryId, int? BrandId, bool? Active, bool? LowStock, int Page, int PageSize) : IRequest<PagedResult<ProductDto>>, IPagedQuery;
