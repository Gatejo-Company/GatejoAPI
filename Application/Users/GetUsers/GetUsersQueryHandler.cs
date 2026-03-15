using API.Application.Shared;
using API.Application.Users;
using API.Domain.Users;
using API.Persistence.Shared;
using API.Persistence.Users.Interfaces;
using MediatR;

namespace API.Application.Users.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>> {
	private readonly IUserRepository _repository;

	public GetUsersQueryHandler(IUserRepository repository) {
		_repository = repository;
	}

	public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken) {
		var filter = new PagedFilter(request.Page, request.PageSize);
		var data = await _repository.GetAllAsync(filter);
		return data.ToPagedResult(filter.PageSize, UserDto.From);
	}
}
