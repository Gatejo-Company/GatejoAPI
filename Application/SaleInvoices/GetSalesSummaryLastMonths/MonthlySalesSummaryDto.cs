namespace API.Application.SaleInvoices.GetSalesSummaryLastMonths;

public record MonthlySalesSummaryDto(
    int Year,
    int Month,
    int InvoiceCount,
    decimal TotalAmount
);
