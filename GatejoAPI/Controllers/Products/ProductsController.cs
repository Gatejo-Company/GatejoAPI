using API.Application.Products.CreateProduct;
using API.Application.Products.DeleteProduct;
using API.Application.Products.GetPriceHistory;
using API.Application.Products.GetProductById;
using API.Application.Products.GetProducts;
using API.Application.Products.GetProductStock;
using API.Application.Products.SetProductActive;
using API.Application.Products.UpdateProduct;
using API.Application.Products.UpdateProductPrice;
using API.Application.Products;
using API.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.API_Clean_Architecture.Controllers.Products;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductsController : ControllerBase {
	private readonly IMediator _mediator;

	public ProductsController(IMediator mediator) {
		_mediator = mediator;
	}

	[HttpGet]
	public async Task<PagedResult<ProductDto>> GetAll(
		[FromQuery] int? categoryId,
		[FromQuery] int? brandId,
		[FromQuery] bool? active,
		[FromQuery] bool? lowStock,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20) {
		return await _mediator.Send(new GetProductsQuery(categoryId, brandId, active, lowStock, page, pageSize));
	}

	[HttpGet("{id:int}")]
	public async Task<ProductDto> GetById(int id) {
		var product = await _mediator.Send(new GetProductByIdQuery(id));
		if (product == null) throw new KeyNotFoundException();
		return product;
	}

	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<ProductDto> Create([FromBody] CreateProductRequest request) {
		return await _mediator.Send(new CreateProductCommand(
			request.Name, request.Description, request.CategoryId, request.BrandId, request.Price, request.MinStock));
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<ProductDto> Update(int id, [FromBody] UpdateProductRequest request) {
		var product = await _mediator.Send(new UpdateProductCommand(
			id, request.Name, request.Description, request.CategoryId, request.BrandId, request.MinStock));
		if (product == null) throw new KeyNotFoundException();
		return product;
	}

	[HttpPatch("{id:int}/price")]
	[Authorize(Roles = "Admin")]
	public async Task<ProductDto> UpdatePrice(int id, [FromBody] UpdatePriceRequest request) {
		var product = await _mediator.Send(new UpdateProductPriceCommand(id, request.Price, request.Reason));
		if (product == null) throw new KeyNotFoundException();
		return product;
	}

	[HttpPatch("{id:int}/active")]
	[Authorize(Roles = "Admin")]
	public async Task SetActive(int id, [FromBody] SetActiveProductRequest request) {
		var ok = await _mediator.Send(new SetProductActiveCommand(id, request.Active));
		if (!ok) throw new KeyNotFoundException();
	}

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task Delete(int id) {
		var deleted = await _mediator.Send(new DeleteProductCommand(id));
		if (!deleted) throw new KeyNotFoundException();
	}

	[HttpGet("{id:int}/price-history")]
	[Authorize(Roles = "Admin")]
	public async Task<PagedResult<PriceHistoryDto>> GetPriceHistory(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20) {
		return await _mediator.Send(new GetPriceHistoryQuery(id, page, pageSize));
	}

	[HttpGet("{id:int}/stock")]
	public async Task<ProductStockDto> GetStock(int id) {
		var stock = await _mediator.Send(new GetProductStockQuery(id));
		return new ProductStockDto(id, stock);
	}
}
