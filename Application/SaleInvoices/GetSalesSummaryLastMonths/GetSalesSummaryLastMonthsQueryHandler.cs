using API.Persistence.SaleInvoices.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.GetSalesSummaryLastMonths;

public class GetSalesSummaryLastMonthsQueryHandler : IRequestHandler<GetSalesSummaryLastMonthsQuery, List<MonthlySalesSummaryDto>> {
    private readonly ISaleInvoiceRepository _repository;

    public GetSalesSummaryLastMonthsQueryHandler(ISaleInvoiceRepository repository) {
        _repository = repository;
    }

    public async Task<List<MonthlySalesSummaryDto>> Handle(GetSalesSummaryLastMonthsQuery request, CancellationToken cancellationToken) {
        var data = await _repository.GetSalesSummaryLastMonthsAsync(request.Months);
        return data.Select(MonthlySalesSummaryDto.From).ToList();
    }
}
