namespace API.Domain.SaleInvoices;

public record MonthlySalesSummaryDto(
    int Year,
    int Month,
    int InvoiceCount,
    decimal TotalAmount
);
