using API.Application.Products;
using API.Application.Shared;
using API.Domain.Products;
using API.Persistence.Products.Interfaces;
using MediatR;

namespace API.Application.Products.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>> {
	private readonly IProductRepository _repository;

	public GetProductsQueryHandler(IProductRepository repository) {
		_repository = repository;
	}

	public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken) {
		var filter = new ProductsFilter(request.CategoryId, request.BrandId, request.Active, request.LowStock, request.Page, request.PageSize);
		var data = await _repository.GetAllAsync(filter);
		return data.ToPagedResult(filter.PageSize, ProductDto.From);
	}
}
