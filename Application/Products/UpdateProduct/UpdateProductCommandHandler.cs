using API.Application.Products;
using API.Domain.Products;
using API.Persistence.Products.Interfaces;
using MediatR;

namespace API.Application.Products.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto?> {
	private readonly IProductRepository _repository;

	public UpdateProductCommandHandler(IProductRepository repository) {
		_repository = repository;
	}

	public async Task<ProductDto?> Handle(UpdateProductCommand request, CancellationToken cancellationToken) {
		var product = await _repository.UpdateAsync(request.Id, request.Name, request.Description, request.CategoryId, request.BrandId, request.MinStock);
		return product == null ? null : ProductDto.From(product);
	}
}
