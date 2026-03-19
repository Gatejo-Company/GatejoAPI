using API.Persistence.SaleInvoices.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.GetSalesSummary12Months;

public class GetSalesSummary12MonthsQueryHandler : IRequestHandler<GetSalesSummary12MonthsQuery, List<MonthlySalesSummaryDto>> {
    private readonly ISaleInvoiceRepository _repository;

    public GetSalesSummary12MonthsQueryHandler(ISaleInvoiceRepository repository) {
        _repository = repository;
    }

    public async Task<List<MonthlySalesSummaryDto>> Handle(GetSalesSummary12MonthsQuery request, CancellationToken cancellationToken) {
        return await _repository.GetSalesSummaryLast12MonthsAsync();
    }
}
