using API.Application.PurchaseInvoices.CreatePurchaseInvoice;
using API.Application.PurchaseInvoices.DeletePurchaseInvoice;
using API.Application.PurchaseInvoices.GetPurchaseInvoiceById;
using API.Application.PurchaseInvoices.GetPurchaseInvoices;
using API.Application.PurchaseInvoices.UpdatePurchasePayment;
using API.Application.PurchaseInvoices;
using API.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.API_Clean_Architecture.Controllers.PurchaseInvoices;

[Route("api/purchase-invoices")]
[ApiController]
[Authorize(Roles = "Admin")]
public class PurchaseInvoicesController : ControllerBase {
	private readonly IMediator _mediator;

	public PurchaseInvoicesController(IMediator mediator) {
		_mediator = mediator;
	}

	[HttpGet]
	public async Task<PagedResult<PurchaseInvoiceDto>> GetAll(
		[FromQuery] int? supplierId,
		[FromQuery] DateOnly? from,
		[FromQuery] DateOnly? to,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20) {
		return await _mediator.Send(new GetPurchaseInvoicesQuery(supplierId, from, to, page, pageSize));
	}

	[HttpGet("{id:int}")]
	public async Task<PurchaseInvoiceDto> GetById(int id) {
		var invoice = await _mediator.Send(new GetPurchaseInvoiceByIdQuery(id));
		if (invoice == null) throw new KeyNotFoundException();
		return invoice;
	}

	[HttpPost]
	public async Task<PurchaseInvoiceDto> Create([FromBody] CreatePurchaseInvoiceRequest request) {
		var items = request.Items.Select(i => new InvoiceItemDto(i.ProductId, i.Quantity, i.UnitPrice)).ToList();
		return await _mediator.Send(new CreatePurchaseInvoiceCommand(
			request.SupplierId, request.Date, request.Notes, items));
	}

	[HttpPatch("{id:int}/payment")]
	public async Task UpdatePayment(int id, [FromBody] UpdatePaymentRequest request) {
		var ok = await _mediator.Send(new UpdatePurchasePaymentCommand(id, request.Paid));
		if (!ok) throw new KeyNotFoundException();
	}

	[HttpDelete("{id:int}")]
	public async Task Delete(int id) {
		var deleted = await _mediator.Send(new DeletePurchaseInvoiceCommand(id));
		if (!deleted) throw new KeyNotFoundException();
	}
}
