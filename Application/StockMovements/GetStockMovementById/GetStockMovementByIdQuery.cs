using API.Application.StockMovements;
using MediatR;

namespace API.Application.StockMovements.GetStockMovementById;

public record GetStockMovementByIdQuery(int Id) : IRequest<StockMovementDto?>;
