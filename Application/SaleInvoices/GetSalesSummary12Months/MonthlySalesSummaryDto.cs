namespace API.Application.SaleInvoices.GetSalesSummary12Months;

public record MonthlySalesSummaryDto(
    int Year,
    int Month,
    int InvoiceCount,
    decimal TotalAmount
);
