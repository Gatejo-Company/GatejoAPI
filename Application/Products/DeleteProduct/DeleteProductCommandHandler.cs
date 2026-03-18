using API.Persistence.Products.Interfaces;
using MediatR;

namespace API.Application.Products.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool> {
	private readonly IProductRepository _repository;

	public DeleteProductCommandHandler(IProductRepository repository) {
		_repository = repository;
	}

	public Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken) =>
		_repository.DeleteAsync(request.Id);
}
