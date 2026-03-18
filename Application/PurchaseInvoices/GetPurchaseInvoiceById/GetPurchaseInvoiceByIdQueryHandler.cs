using API.Application.PurchaseInvoices;
using API.Domain.PurchaseInvoices;
using API.Persistence.PurchaseInvoices.Interfaces;
using MediatR;

namespace API.Application.PurchaseInvoices.GetPurchaseInvoiceById;

public class GetPurchaseInvoiceByIdQueryHandler : IRequestHandler<GetPurchaseInvoiceByIdQuery, PurchaseInvoiceDto?> {
	private readonly IPurchaseInvoiceRepository _repository;

	public GetPurchaseInvoiceByIdQueryHandler(IPurchaseInvoiceRepository repository) {
		_repository = repository;
	}

	public async Task<PurchaseInvoiceDto?> Handle(GetPurchaseInvoiceByIdQuery request, CancellationToken cancellationToken) {
		var invoice = await _repository.GetByIdAsync(request.Id);
		return invoice == null ? null : PurchaseInvoiceDto.From(invoice);
	}
}
