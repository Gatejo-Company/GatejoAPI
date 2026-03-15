using API.Application.PurchaseInvoices;
using API.Application.Shared;
using MediatR;

namespace API.Application.PurchaseInvoices.GetPurchaseInvoices;

public record GetPurchaseInvoicesQuery(int? SupplierId, DateOnly? From, DateOnly? To, int Page, int PageSize) : IRequest<PagedResult<PurchaseInvoiceDto>>, IPagedQuery;
