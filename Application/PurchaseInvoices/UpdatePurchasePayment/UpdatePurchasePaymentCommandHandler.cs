using API.Persistence.PurchaseInvoices.Interfaces;
using MediatR;

namespace API.Application.PurchaseInvoices.UpdatePurchasePayment;

public class UpdatePurchasePaymentCommandHandler : IRequestHandler<UpdatePurchasePaymentCommand, bool> {
	private readonly IPurchaseInvoiceRepository _repository;

	public UpdatePurchasePaymentCommandHandler(IPurchaseInvoiceRepository repository) {
		_repository = repository;
	}

	public Task<bool> Handle(UpdatePurchasePaymentCommand request, CancellationToken cancellationToken) =>
		_repository.UpdatePaymentAsync(request.Id, request.Paid);
}
