using API.Domain.StockMovements;

namespace API.Application.StockMovements;

public record StockMovementDto(
	int Id,
	int ProductId,
	string ProductName,
	int TypeId,
	string TypeName,
	int Quantity,
	int? ReferenceId,
	string? Notes,
	DateTime CreatedAt) {
	public static StockMovementDto From(StockMovement sm) => new(
		sm.Id, sm.ProductId, sm.ProductName,
		sm.TypeId, sm.TypeName,
		sm.Quantity, sm.ReferenceId,
		sm.Notes, sm.CreatedAt);
}
