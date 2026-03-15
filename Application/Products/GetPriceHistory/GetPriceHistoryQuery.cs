using API.Application.Products;
using API.Application.Shared;
using MediatR;

namespace API.Application.Products.GetPriceHistory;

public record GetPriceHistoryQuery(int ProductId, int Page, int PageSize) : IRequest<PagedResult<PriceHistoryDto>>, IPagedQuery;
