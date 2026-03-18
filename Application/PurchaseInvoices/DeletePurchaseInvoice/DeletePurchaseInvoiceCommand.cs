using MediatR;

namespace API.Application.PurchaseInvoices.DeletePurchaseInvoice;

public record DeletePurchaseInvoiceCommand(int Id) : IRequest<bool>;
