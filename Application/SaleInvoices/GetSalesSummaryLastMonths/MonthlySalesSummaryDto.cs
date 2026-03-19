using API.Domain.SaleInvoices;

namespace API.Application.SaleInvoices.GetSalesSummaryLastMonths;

public record MonthlySalesSummaryDto(int Year, int Month, int InvoiceCount, decimal TotalAmount) {
    public static MonthlySalesSummaryDto From(MonthlySalesSummary s) =>
        new(s.Year, s.Month, s.InvoiceCount, s.TotalAmount);
}
