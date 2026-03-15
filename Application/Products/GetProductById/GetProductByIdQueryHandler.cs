using API.Application.Products;
using API.Domain.Products;
using API.Persistence.Products.Interfaces;
using MediatR;

namespace API.Application.Products.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?> {
	private readonly IProductRepository _repository;

	public GetProductByIdQueryHandler(IProductRepository repository) {
		_repository = repository;
	}

	public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken) {
		var product = await _repository.GetByIdAsync(request.Id);
		return product == null ? null : ProductDto.From(product);
	}
}
