using API.Persistence.Products.Interfaces;
using MediatR;

namespace API.Application.Products.GetProductStock;

public class GetProductStockQueryHandler : IRequestHandler<GetProductStockQuery, int> {
	private readonly IProductRepository _repository;

	public GetProductStockQueryHandler(IProductRepository repository) {
		_repository = repository;
	}

	public Task<int> Handle(GetProductStockQuery request, CancellationToken cancellationToken) =>
		_repository.GetCurrentStockAsync(request.ProductId);
}
