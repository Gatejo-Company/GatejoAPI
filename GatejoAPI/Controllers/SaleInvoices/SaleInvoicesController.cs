using API.Application.SaleInvoices;
using API.Application.SaleInvoices.CreateSaleInvoice;
using API.Application.SaleInvoices.DeleteSaleInvoice;
using API.Application.SaleInvoices.GetPendingCredit;
using API.Application.SaleInvoices.GetSaleInvoiceById;
using API.Application.SaleInvoices.GetSaleInvoices;
using API.Application.SaleInvoices.GetSalesSummaryLastMonths;
using API.Application.SaleInvoices.MarkSaleAsPaid;
using API.Domain.SaleInvoices;
using API.Application.SaleInvoices.ReverseSaleInvoice;
using API.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.API_Clean_Architecture.Controllers.SaleInvoices;

[Route("api/sale-invoices")]
[ApiController]
[Authorize]
public class SaleInvoicesController : ControllerBase {
    private readonly IMediator _mediator;

    public SaleInvoicesController(IMediator mediator) {
        _mediator = mediator;
    }

    [HttpGet("summary/last-months")]
    public async Task<List<MonthlySalesSummaryDto>> GetSalesSummaryLastMonths([FromQuery] int months = 12) {
        return await _mediator.Send(new GetSalesSummaryLastMonthsQuery(months));
    }

    [HttpGet]
    public async Task<PagedResult<SaleInvoiceDto>> GetAll(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] bool? onCredit,
        [FromQuery] bool? paid,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20) {
        return await _mediator.Send(new GetSaleInvoicesQuery(from, to, onCredit, paid, page, pageSize));
    }

    [HttpGet("pending-credit")]
    [Authorize(Roles = "Admin")]
    public async Task<PagedResult<SaleInvoiceDto>> GetPendingCredit([FromQuery] int page = 1, [FromQuery] int pageSize = 20) {
        return await _mediator.Send(new GetPendingCreditQuery(page, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<SaleInvoiceDto> GetById(int id) {
        var invoice = await _mediator.Send(new GetSaleInvoiceByIdQuery(id));
        if (invoice == null) throw new KeyNotFoundException();
        return invoice;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<SaleInvoiceDto> Create([FromBody] CreateSaleInvoiceRequest request) {
        var items = request.Items.Select(i => new SaleItemDto(i.ProductId, i.Quantity, i.UnitPrice)).ToList();
        return await _mediator.Send(new CreateSaleInvoiceCommand(request.Date, request.OnCredit, request.Notes, items));
    }

    [HttpPatch("{id:int}/pay")]
    [Authorize(Roles = "Admin")]
    public async Task MarkAsPaid(int id) {
        var ok = await _mediator.Send(new MarkSaleAsPaidCommand(id));
        if (!ok) throw new KeyNotFoundException();
    }

    [HttpPost("{id:int}/reverse")]
    [Authorize(Roles = "Admin")]
    public async Task<SaleInvoiceDto> Reverse(int id) {
        return await _mediator.Send(new ReverseSaleInvoiceCommand(id));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task Delete(int id) {
        var deleted = await _mediator.Send(new DeleteSaleInvoiceCommand(id));
        if (!deleted) throw new KeyNotFoundException();
    }
}
