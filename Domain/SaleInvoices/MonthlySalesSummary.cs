namespace API.Domain.SaleInvoices;

public record MonthlySalesSummary(
    int Year,
    int Month,
    int InvoiceCount,
    decimal TotalAmount
);
