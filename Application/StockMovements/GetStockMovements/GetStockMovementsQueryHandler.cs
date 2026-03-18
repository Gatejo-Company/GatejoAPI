using API.Application.Shared;
using API.Application.StockMovements;
using API.Domain.StockMovements;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.StockMovements.GetStockMovements;

public class GetStockMovementsQueryHandler : IRequestHandler<GetStockMovementsQuery, PagedResult<StockMovementDto>> {
	private readonly IStockMovementRepository _repository;

	public GetStockMovementsQueryHandler(IStockMovementRepository repository) {
		_repository = repository;
	}

	public async Task<PagedResult<StockMovementDto>> Handle(GetStockMovementsQuery request, CancellationToken cancellationToken) {
		var filter = new StockMovementsFilter(request.ProductId, request.TypeId, request.From, request.To, request.Page, request.PageSize);
		var data = await _repository.GetAllAsync(filter);
		return data.ToPagedResult(filter.PageSize, StockMovementDto.From);
	}
}
