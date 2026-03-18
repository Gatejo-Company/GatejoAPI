using API.Domain.StockMovements;

namespace API.Application.StockMovements;

public record MovementTypeDto(int Id, string Name) {
	public static MovementTypeDto From(MovementType mt) => new(mt.Id, mt.Name);
}
