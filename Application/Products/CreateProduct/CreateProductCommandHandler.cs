using API.Application.Products;
using API.Domain.Products;
using API.Persistence.Products.Interfaces;
using MediatR;

namespace API.Application.Products.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto> {
	private readonly IProductRepository _repository;

	public CreateProductCommandHandler(IProductRepository repository) {
		_repository = repository;
	}

	public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken) {
		var product = await _repository.CreateAsync(request.Name, request.Description, request.CategoryId, request.BrandId, request.Price, request.MinStock);
		return ProductDto.From(product);
	}
}
