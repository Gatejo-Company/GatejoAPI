using API.Application.Roles.GetRoleById;
using API.Application.Roles.GetRoles;
using API.Application.Roles;
using API.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.API_Clean_Architecture.Controllers.Roles;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RolesController : ControllerBase {
	private readonly IMediator _mediator;

	public RolesController(IMediator mediator) {
		_mediator = mediator;
	}

	[HttpGet]
	public async Task<PagedResult<RoleDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20) {
		return await _mediator.Send(new GetRolesQuery(page, pageSize));
	}

	[HttpGet("{id:int}")]
	public async Task<RoleDto> GetById(int id) {
		var role = await _mediator.Send(new GetRoleByIdQuery(id));
		if (role == null) throw new KeyNotFoundException();
		return role;
	}
}
