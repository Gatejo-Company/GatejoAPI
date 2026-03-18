using API.Application.SaleInvoices;
using API.Application.Shared;
using API.Domain.SaleInvoices;
using API.Persistence.SaleInvoices.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.GetSaleInvoices;

public class GetSaleInvoicesQueryHandler : IRequestHandler<GetSaleInvoicesQuery, PagedResult<SaleInvoiceDto>> {
	private readonly ISaleInvoiceRepository _repository;

	public GetSaleInvoicesQueryHandler(ISaleInvoiceRepository repository) {
		_repository = repository;
	}

	public async Task<PagedResult<SaleInvoiceDto>> Handle(GetSaleInvoicesQuery request, CancellationToken cancellationToken) {
		var filter = new SaleInvoicesFilter(request.From, request.To, request.OnCredit, request.Paid, request.Page, request.PageSize);
		var data = await _repository.GetAllAsync(filter);
		return data.ToPagedResult(filter.PageSize, SaleInvoiceDto.From);
	}
}
