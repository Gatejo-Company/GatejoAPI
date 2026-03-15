using API.Application.Products;
using API.Application.Shared;
using API.Domain.Products;
using API.Persistence.Products.Interfaces;
using MediatR;

namespace API.Application.Products.GetPriceHistory;

public class GetPriceHistoryQueryHandler : IRequestHandler<GetPriceHistoryQuery, PagedResult<PriceHistoryDto>> {
	private readonly IProductRepository _repository;

	public GetPriceHistoryQueryHandler(IProductRepository repository) {
		_repository = repository;
	}

	public async Task<PagedResult<PriceHistoryDto>> Handle(GetPriceHistoryQuery request, CancellationToken cancellationToken) {
		var filter = new PriceHistoryFilter(request.ProductId, request.Page, request.PageSize);
		var data = await _repository.GetPriceHistoryAsync(filter);
		return data.ToPagedResult(filter.PageSize, PriceHistoryDto.From);
	}
}
