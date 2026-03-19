using API.Domain.SaleInvoices;
using MediatR;

namespace API.Application.SaleInvoices.GetSalesSummaryLastMonths;

public record GetSalesSummaryLastMonthsQuery(int Months) : IRequest<List<MonthlySalesSummaryDto>>;
