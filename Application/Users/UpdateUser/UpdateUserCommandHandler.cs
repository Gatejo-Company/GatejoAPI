using API.Application.Users;
using API.Domain.Users;
using API.Persistence.Users.Interfaces;
using MediatR;

namespace API.Application.Users.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto?> {
	private readonly IUserRepository _repository;

	public UpdateUserCommandHandler(IUserRepository repository) {
		_repository = repository;
	}

	public async Task<UserDto?> Handle(UpdateUserCommand request, CancellationToken cancellationToken) {
		var user = await _repository.UpdateAsync(request.Id, request.Name, request.Email, request.RoleId);
		return user == null ? null : UserDto.From(user);
	}
}
