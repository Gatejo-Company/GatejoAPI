using API.Application.SaleInvoices;
using MediatR;

namespace API.Application.SaleInvoices.CreateSaleInvoice;

public record SaleItemDto(int ProductId, int Quantity, decimal UnitPrice);

public record CreateSaleInvoiceCommand(
	int IdReverdesInvoice,
	DateOnly Date,
	bool OnCredit,
	bool Reversed,
	string? Notes,
	List<SaleItemDto> Items
) : IRequest<SaleInvoiceDto>;
