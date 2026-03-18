using System.Net;
using System.Security.Claims;
using API.Application.Shared;
using API.Application.Users.DeleteUser;
using API.Application.Users.GetUserById;
using API.Application.Users.GetUsers;
using API.Application.Users.SetUserActive;
using API.Application.Users.UpdateUser;
using API.Domain.Exceptions;
using API.Application.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.API_Clean_Architecture.Controllers.Users;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase {
	private readonly IMediator _mediator;

	public UsersController(IMediator mediator) {
		_mediator = mediator;
	}

	[HttpGet]
	[Authorize(Roles = "Admin")]
	public async Task<PagedResult<UserDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20) {
		return await _mediator.Send(new GetUsersQuery(page, pageSize));
	}

	[HttpGet("{id:int}")]
	public async Task<UserDto> GetById(int id) {
		if (!IsAdminOrOwner(id)) throw new DomainException(HttpStatusCode.Forbidden, "Forbidden", "Access denied.");
		var user = await _mediator.Send(new GetUserByIdQuery(id));
		if (user == null) throw new KeyNotFoundException();
		return user;
	}

	[HttpPut("{id:int}")]
	public async Task<UserDto> Update(int id, [FromBody] UpdateUserRequest request) {
		if (!IsAdminOrOwner(id)) throw new DomainException(HttpStatusCode.Forbidden, "Forbidden", "Access denied.");
		var user = await _mediator.Send(new UpdateUserCommand(id, request.Name, request.Email, request.RoleId));
		if (user == null) throw new KeyNotFoundException();
		return user;
	}

	[HttpPatch("{id:int}/active")]
	[Authorize(Roles = "Admin")]
	public async Task SetActive(int id, [FromBody] SetActiveRequest request) {
		var ok = await _mediator.Send(new SetUserActiveCommand(id, request.Active));
		if (!ok) throw new KeyNotFoundException();
	}

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task Delete(int id) {
		var deleted = await _mediator.Send(new DeleteUserCommand(id));
		if (!deleted) throw new KeyNotFoundException();
	}

	private bool IsAdminOrOwner(int resourceUserId) {
		var role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
		if (role == "Admin") return true;
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		return userId == resourceUserId.ToString();
	}
}
