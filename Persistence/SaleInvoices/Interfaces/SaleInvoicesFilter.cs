namespace API.Persistence.SaleInvoices.Interfaces;

public record SaleInvoicesFilter(DateOnly? From, DateOnly? To, bool? OnCredit, bool? Paid, int Page, int PageSize);
