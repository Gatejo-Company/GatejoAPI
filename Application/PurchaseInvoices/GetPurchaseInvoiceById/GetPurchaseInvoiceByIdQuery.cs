using API.Application.PurchaseInvoices;
using MediatR;

namespace API.Application.PurchaseInvoices.GetPurchaseInvoiceById;

public record GetPurchaseInvoiceByIdQuery(int Id) : IRequest<PurchaseInvoiceDto?>;
