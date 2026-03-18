using API.Persistence.SaleInvoices.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.MarkSaleAsPaid;

public class MarkSaleAsPaidCommandHandler : IRequestHandler<MarkSaleAsPaidCommand, bool> {
	private readonly ISaleInvoiceRepository _repository;

	public MarkSaleAsPaidCommandHandler(ISaleInvoiceRepository repository) {
		_repository = repository;
	}

	public Task<bool> Handle(MarkSaleAsPaidCommand request, CancellationToken cancellationToken) =>
		_repository.MarkAsPaidAsync(request.Id);
}
