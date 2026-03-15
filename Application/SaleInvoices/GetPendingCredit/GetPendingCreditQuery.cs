using API.Application.SaleInvoices;
using API.Application.Shared;
using MediatR;

namespace API.Application.SaleInvoices.GetPendingCredit;

public record GetPendingCreditQuery(int Page, int PageSize) : IRequest<PagedResult<SaleInvoiceDto>>, IPagedQuery;
