using API.Persistence.Products.Interfaces;
using MediatR;

namespace API.Application.Products.SetProductActive;

public class SetProductActiveCommandHandler : IRequestHandler<SetProductActiveCommand, bool> {
	private readonly IProductRepository _repository;

	public SetProductActiveCommandHandler(IProductRepository repository) {
		_repository = repository;
	}

	public Task<bool> Handle(SetProductActiveCommand request, CancellationToken cancellationToken) =>
		_repository.SetActiveAsync(request.Id, request.Active);
}
