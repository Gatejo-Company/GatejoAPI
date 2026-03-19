using API.Domain.SaleInvoices;
using API.Persistence.Shared;

namespace API.Persistence.SaleInvoices.Interfaces;

public interface ISaleInvoiceRepository {
	Task<PagedData<SaleInvoice>> GetAllAsync(SaleInvoicesFilter filter);
	Task<SaleInvoice?> GetByIdAsync(int id);
	Task<int> CreateAsync(DateOnly date, bool onCredit, bool reversed, decimal total, string? notes);
	Task AddItemAsync(int invoiceId, int productId, int quantity, decimal unitPrice, decimal subtotal);
	Task<bool> MarkAsPaidAsync(int id);
	Task<bool> ReverseAsync(int id);
	Task<bool> DeleteAsync(int id);
	Task<PagedData<SaleInvoice>> GetPendingCreditAsync(PagedFilter filter);
	Task<List<MonthlySalesSummaryDto>> GetSalesSummaryLastMonthsAsync(int months);
}
