using API.Persistence.Users.Interfaces;
using MediatR;

namespace API.Application.Users.SetUserActive;

public class SetUserActiveCommandHandler : IRequestHandler<SetUserActiveCommand, bool> {
	private readonly IUserRepository _repository;

	public SetUserActiveCommandHandler(IUserRepository repository) {
		_repository = repository;
	}

	public Task<bool> Handle(SetUserActiveCommand request, CancellationToken cancellationToken) =>
		_repository.SetActiveAsync(request.Id, request.Active);
}
