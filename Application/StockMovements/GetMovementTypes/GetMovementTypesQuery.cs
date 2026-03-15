using API.Application.Shared;
using API.Application.StockMovements;
using MediatR;

namespace API.Application.StockMovements.GetMovementTypes;

public record GetMovementTypesQuery(int Page, int PageSize) : IRequest<PagedResult<MovementTypeDto>>, IPagedQuery;
