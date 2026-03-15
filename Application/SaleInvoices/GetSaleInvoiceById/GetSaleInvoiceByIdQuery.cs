using API.Application.SaleInvoices;
using MediatR;

namespace API.Application.SaleInvoices.GetSaleInvoiceById;

public record GetSaleInvoiceByIdQuery(int Id) : IRequest<SaleInvoiceDto?>;
