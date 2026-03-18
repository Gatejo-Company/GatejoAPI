using API.Application.Products;
using API.Domain.Products;
using API.Persistence.Products.Interfaces;
using MediatR;

namespace API.Application.Products.UpdateProductPrice;

public class UpdateProductPriceCommandHandler : IRequestHandler<UpdateProductPriceCommand, ProductDto?> {
	private readonly IProductRepository _repository;

	public UpdateProductPriceCommandHandler(IProductRepository repository) {
		_repository = repository;
	}

	public async Task<ProductDto?> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken) {
		var product = await _repository.UpdatePriceAsync(request.Id, request.Price, request.Reason);
		return product == null ? null : ProductDto.From(product);
	}
}
