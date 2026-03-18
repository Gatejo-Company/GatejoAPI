using MediatR;

namespace API.Application.SaleInvoices.MarkSaleAsPaid;

public record MarkSaleAsPaidCommand(int Id) : IRequest<bool>;
