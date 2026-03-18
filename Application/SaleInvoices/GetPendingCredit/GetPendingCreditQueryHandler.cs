using API.Application.SaleInvoices;
using API.Application.Shared;
using API.Domain.SaleInvoices;
using API.Persistence.Shared;
using API.Persistence.SaleInvoices.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.GetPendingCredit;

public class GetPendingCreditQueryHandler : IRequestHandler<GetPendingCreditQuery, PagedResult<SaleInvoiceDto>> {
	private readonly ISaleInvoiceRepository _repository;

	public GetPendingCreditQueryHandler(ISaleInvoiceRepository repository) {
		_repository = repository;
	}

	public async Task<PagedResult<SaleInvoiceDto>> Handle(GetPendingCreditQuery request, CancellationToken cancellationToken) {
		var filter = new PagedFilter(request.Page, request.PageSize);
		var data = await _repository.GetPendingCreditAsync(filter);
		return data.ToPagedResult(filter.PageSize, SaleInvoiceDto.From);
	}
}
