using API.Application.Shared;
using API.Application.StockMovements;
using MediatR;

namespace API.Application.StockMovements.GetStockMovements;

public record GetStockMovementsQuery(int? ProductId, int? TypeId, DateTime? From, DateTime? To, int Page, int PageSize) : IRequest<PagedResult<StockMovementDto>>, IPagedQuery;
