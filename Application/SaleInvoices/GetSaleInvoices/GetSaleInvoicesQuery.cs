using API.Application.SaleInvoices;
using API.Application.Shared;
using MediatR;

namespace API.Application.SaleInvoices.GetSaleInvoices;

public record GetSaleInvoicesQuery(DateOnly? From, DateOnly? To, bool? OnCredit, bool? Paid, int Page, int PageSize) : IRequest<PagedResult<SaleInvoiceDto>>, IPagedQuery;
