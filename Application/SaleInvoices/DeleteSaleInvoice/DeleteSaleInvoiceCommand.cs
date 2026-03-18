using MediatR;

namespace API.Application.SaleInvoices.DeleteSaleInvoice;

public record DeleteSaleInvoiceCommand(int Id) : IRequest<bool>;
