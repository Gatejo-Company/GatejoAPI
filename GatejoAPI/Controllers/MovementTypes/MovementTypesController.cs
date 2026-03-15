using API.Application.StockMovements.GetMovementTypes;
using API.Application.StockMovements;
using API.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.API_Clean_Architecture.Controllers.MovementTypes;

[Route("api/movement-types")]
[ApiController]
[Authorize]
public class MovementTypesController : ControllerBase {
	private readonly IMediator _mediator;

	public MovementTypesController(IMediator mediator) {
		_mediator = mediator;
	}

	[HttpGet]
	public async Task<PagedResult<MovementTypeDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20) {
		return await _mediator.Send(new GetMovementTypesQuery(page, pageSize));
	}
}
