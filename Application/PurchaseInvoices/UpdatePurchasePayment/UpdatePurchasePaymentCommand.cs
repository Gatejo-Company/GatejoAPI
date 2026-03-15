using MediatR;

namespace API.Application.PurchaseInvoices.UpdatePurchasePayment;

public record UpdatePurchasePaymentCommand(int Id, decimal Paid) : IRequest<bool>;
