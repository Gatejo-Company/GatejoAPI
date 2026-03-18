using API.Application.Users;
using API.Domain.Users;
using API.Persistence.Users.Interfaces;
using MediatR;

namespace API.Application.Users.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?> {
	private readonly IUserRepository _repository;

	public GetUserByIdQueryHandler(IUserRepository repository) {
		_repository = repository;
	}

	public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken) {
		var user = await _repository.GetByIdAsync(request.Id);
		return user == null ? null : UserDto.From(user);
	}
}
