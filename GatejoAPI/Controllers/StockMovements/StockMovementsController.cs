using API.Application.StockMovements.CreateStockMovement;
using API.Application.StockMovements.GetStockMovementById;
using API.Application.StockMovements.GetStockMovements;
using API.Application.StockMovements;
using API.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.API_Clean_Architecture.Controllers.StockMovements;

[Route("api/stock-movements")]
[ApiController]
[Authorize(Roles = "Admin")]
public class StockMovementsController : ControllerBase {
	private readonly IMediator _mediator;

	public StockMovementsController(IMediator mediator) {
		_mediator = mediator;
	}

	[HttpGet]
	public async Task<PagedResult<StockMovementDto>> GetAll(
		[FromQuery] int? productId,
		[FromQuery] int? typeId,
		[FromQuery] DateTime? from,
		[FromQuery] DateTime? to,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20) {
		return await _mediator.Send(new GetStockMovementsQuery(productId, typeId, from, to, page, pageSize));
	}

	[HttpGet("{id:int}")]
	public async Task<StockMovementDto> GetById(int id) {
		var movement = await _mediator.Send(new GetStockMovementByIdQuery(id));
		if (movement == null) throw new KeyNotFoundException();
		return movement;
	}

	[HttpPost]
	public async Task<StockMovementDto> Create([FromBody] StockMovementRequest request) {
		return await _mediator.Send(new CreateStockMovementCommand(
			request.ProductId, request.TypeId, request.Quantity, request.Notes));
	}
}
