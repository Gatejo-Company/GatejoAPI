using API.Application.Roles;
using API.Domain.Roles;
using API.Persistence.Roles.Interfaces;
using MediatR;

namespace API.Application.Roles.GetRoleById;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto?> {
	private readonly IRoleRepository _repository;

	public GetRoleByIdQueryHandler(IRoleRepository repository) {
		_repository = repository;
	}

	public async Task<RoleDto?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken) {
		var role = await _repository.GetByIdAsync(request.Id);
		return role == null ? null : RoleDto.From(role);
	}
}
