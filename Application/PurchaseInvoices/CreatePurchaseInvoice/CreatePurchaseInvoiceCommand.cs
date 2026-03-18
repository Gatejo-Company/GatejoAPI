using API.Application.PurchaseInvoices;
using MediatR;

namespace API.Application.PurchaseInvoices.CreatePurchaseInvoice;

public record InvoiceItemDto(int ProductId, int Quantity, decimal UnitPrice);

public record CreatePurchaseInvoiceCommand(
	int SupplierId,
	DateOnly Date,
	string? Notes,
	List<InvoiceItemDto> Items
) : IRequest<PurchaseInvoiceDto>;
