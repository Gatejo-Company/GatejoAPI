using API.Domain.PurchaseInvoices;
using API.Persistence.Shared;

namespace API.Persistence.PurchaseInvoices.Interfaces;

public interface IPurchaseInvoiceRepository {
	Task<PagedData<PurchaseInvoice>> GetAllAsync(PurchaseInvoicesFilter filter);
	Task<PurchaseInvoice?> GetByIdAsync(int id);
	Task<int> CreateAsync(int supplierId, DateOnly date, decimal total, string? notes);
	Task AddItemAsync(int invoiceId, int productId, int quantity, decimal unitPrice);
	Task<bool> UpdatePaymentAsync(int id, decimal paid);
	Task<bool> DeleteAsync(int id);
}
