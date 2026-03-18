using API.Application.StockMovements;
using API.Domain.StockMovements;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.StockMovements.GetStockMovementById;

public class GetStockMovementByIdQueryHandler : IRequestHandler<GetStockMovementByIdQuery, StockMovementDto?> {
	private readonly IStockMovementRepository _repository;

	public GetStockMovementByIdQueryHandler(IStockMovementRepository repository) {
		_repository = repository;
	}

	public async Task<StockMovementDto?> Handle(GetStockMovementByIdQuery request, CancellationToken cancellationToken) {
		var movement = await _repository.GetByIdAsync(request.Id);
		return movement == null ? null : StockMovementDto.From(movement);
	}
}
