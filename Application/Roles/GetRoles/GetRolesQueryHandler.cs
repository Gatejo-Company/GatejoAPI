using API.Application.Roles;
using API.Application.Shared;
using API.Domain.Roles;
using API.Persistence.Roles.Interfaces;
using API.Persistence.Shared;
using MediatR;

namespace API.Application.Roles.GetRoles;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, PagedResult<RoleDto>> {
	private readonly IRoleRepository _repository;

	public GetRolesQueryHandler(IRoleRepository repository) {
		_repository = repository;
	}

	public async Task<PagedResult<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken) {
		var filter = new PagedFilter(request.Page, request.PageSize);
		var data = await _repository.GetAllAsync(filter);
		return data.ToPagedResult(filter.PageSize, RoleDto.From);
	}
}
