using API.Application.StockMovements;
using API.Domain.StockMovements;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.StockMovements.CreateStockMovement;

public class CreateStockMovementCommandHandler : IRequestHandler<CreateStockMovementCommand, StockMovementDto> {
	private readonly IStockMovementRepository _repository;

	public CreateStockMovementCommandHandler(IStockMovementRepository repository) {
		_repository = repository;
	}

	public async Task<StockMovementDto> Handle(CreateStockMovementCommand request, CancellationToken cancellationToken) {
		var movement = await _repository.CreateManualAdjustmentAsync(request.ProductId, request.TypeId, request.Quantity, request.Notes);
		return StockMovementDto.From(movement);
	}
}
