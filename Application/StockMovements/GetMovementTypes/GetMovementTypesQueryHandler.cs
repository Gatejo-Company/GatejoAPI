using API.Application.Shared;
using API.Application.StockMovements;
using API.Domain.StockMovements;
using API.Persistence.Shared;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.StockMovements.GetMovementTypes;

public class GetMovementTypesQueryHandler : IRequestHandler<GetMovementTypesQuery, PagedResult<MovementTypeDto>> {
	private readonly IMovementTypeRepository _repository;

	public GetMovementTypesQueryHandler(IMovementTypeRepository repository) {
		_repository = repository;
	}

	public async Task<PagedResult<MovementTypeDto>> Handle(GetMovementTypesQuery request, CancellationToken cancellationToken) {
		var filter = new PagedFilter(request.Page, request.PageSize);
		var data = await _repository.GetAllAsync(filter);
		return data.ToPagedResult(filter.PageSize, MovementTypeDto.From);
	}
}
