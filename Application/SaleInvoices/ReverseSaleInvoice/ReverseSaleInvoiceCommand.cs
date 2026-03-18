using API.Application.SaleInvoices;
using MediatR;

namespace API.Application.SaleInvoices.ReverseSaleInvoice;

public record ReverseSaleInvoiceCommand(int Id) : IRequest<SaleInvoiceDto>;
