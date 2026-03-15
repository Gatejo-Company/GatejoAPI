using API.Application.StockMovements;
using MediatR;

namespace API.Application.StockMovements.CreateStockMovement;

public record CreateStockMovementCommand(int ProductId, int TypeId, int Quantity, string? Notes) : IRequest<StockMovementDto>;
