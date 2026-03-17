using MediatR;

namespace API.Application.SaleInvoices.CreateSaleInvoice;

public record SaleItemDto(int ProductId, int Quantity, decimal UnitPrice);

public record CreateSaleInvoiceCommand(
    DateOnly Date,
    bool OnCredit,
    string? Notes,
    List<SaleItemDto> Items
) : IRequest<SaleInvoiceDto>;
