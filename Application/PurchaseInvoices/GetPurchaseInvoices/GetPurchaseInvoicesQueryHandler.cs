using API.Application.PurchaseInvoices;
using API.Application.Shared;
using API.Domain.PurchaseInvoices;
using API.Persistence.PurchaseInvoices.Interfaces;
using MediatR;

namespace API.Application.PurchaseInvoices.GetPurchaseInvoices;

public class GetPurchaseInvoicesQueryHandler : IRequestHandler<GetPurchaseInvoicesQuery, PagedResult<PurchaseInvoiceDto>> {
	private readonly IPurchaseInvoiceRepository _repository;

	public GetPurchaseInvoicesQueryHandler(IPurchaseInvoiceRepository repository) {
		_repository = repository;
	}

	public async Task<PagedResult<PurchaseInvoiceDto>> Handle(GetPurchaseInvoicesQuery request, CancellationToken cancellationToken) {
		var filter = new PurchaseInvoicesFilter(request.SupplierId, request.From, request.To, request.Page, request.PageSize);
		var data = await _repository.GetAllAsync(filter);
		return data.ToPagedResult(filter.PageSize, PurchaseInvoiceDto.From);
	}
}
