using API.Application.SaleInvoices;
using API.Domain.SaleInvoices;
using API.Persistence.SaleInvoices.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.GetSaleInvoiceById;

public class GetSaleInvoiceByIdQueryHandler : IRequestHandler<GetSaleInvoiceByIdQuery, SaleInvoiceDto?> {
	private readonly ISaleInvoiceRepository _repository;

	public GetSaleInvoiceByIdQueryHandler(ISaleInvoiceRepository repository) {
		_repository = repository;
	}

	public async Task<SaleInvoiceDto?> Handle(GetSaleInvoiceByIdQuery request, CancellationToken cancellationToken) {
		var invoice = await _repository.GetByIdAsync(request.Id);
		return invoice == null ? null : SaleInvoiceDto.From(invoice);
	}
}
