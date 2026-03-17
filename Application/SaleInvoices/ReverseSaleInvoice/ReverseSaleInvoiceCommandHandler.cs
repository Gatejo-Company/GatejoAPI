using API.DataAccess.Interfaces;
using API.Domain.StockMovements;
using API.Persistence.SaleInvoices.Interfaces;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.ReverseSaleInvoice;

public class ReverseSaleInvoiceCommandHandler : IRequestHandler<ReverseSaleInvoiceCommand, SaleInvoiceDto> {
    private readonly ISaleInvoiceRepository _repository;
    private readonly IStockMovementRepository _stockRepository;
    private readonly ICConnection _connection;

    public ReverseSaleInvoiceCommandHandler(
        ISaleInvoiceRepository repository,
        IStockMovementRepository stockRepository,
        ICConnection connection) {
        _repository = repository;
        _stockRepository = stockRepository;
        _connection = connection;
    }

    public async Task<SaleInvoiceDto> Handle(ReverseSaleInvoiceCommand request, CancellationToken cancellationToken) {
        var original = await _repository.GetByIdAsync(request.Id);

        if (original == null)
            throw new KeyNotFoundException($"Factura #{request.Id} No Encontrada.");

        if (original.Reversed)
            throw new InvalidOperationException($"Factura #{request.Id} Ya se encuentra Anulada.");

        var newTotal = -original.Total;

        await _connection.BeginTransaction();
        try {
            var newInvoiceId = await _repository.CreateAsync(original.Date, original.OnCredit, false, newTotal, original.Notes);

            foreach (var item in original.Items) {
                await _repository.AddItemAsync(newInvoiceId, item.ProductId, -item.Quantity, -item.UnitPrice, -item.Subtotal);

                string note = $"Anulación de la factura #{request.Id} → Nueva factura #{newInvoiceId}";

                // stockDelta = item.Quantity (already signed): undoes the original stock movement
                await _stockRepository.CreateForInvoiceAsync(
                    MovementTypeNames.Sale,
                    item.ProductId,
                    item.Quantity,
                    newInvoiceId,
                    note);
            }

            await _repository.ReverseAsync(request.Id);

            await _connection.CommitTransaction();
            return SaleInvoiceDto.From((await _repository.GetByIdAsync(newInvoiceId))!);
        } catch {
            await _connection.CancelTransaction();
            throw;
        }
    }
}
