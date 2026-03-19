using MediatR;

namespace API.Application.SaleInvoices.GetSalesSummary12Months;

public record GetSalesSummary12MonthsQuery : IRequest<List<MonthlySalesSummaryDto>>;
