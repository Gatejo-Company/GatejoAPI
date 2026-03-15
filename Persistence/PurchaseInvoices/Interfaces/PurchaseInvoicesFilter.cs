namespace API.Persistence.PurchaseInvoices.Interfaces;

public record PurchaseInvoicesFilter(int? SupplierId, DateOnly? From, DateOnly? To, int Page, int PageSize);
