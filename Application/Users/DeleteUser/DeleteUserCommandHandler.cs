using API.Persistence.Users.Interfaces;
using MediatR;

namespace API.Application.Users.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool> {
	private readonly IUserRepository _repository;

	public DeleteUserCommandHandler(IUserRepository repository) {
		_repository = repository;
	}

	public Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken) =>
		_repository.DeleteAsync(request.Id);
}
