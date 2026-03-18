using API.DataAccess.Interfaces;
using API.Persistence.SaleInvoices.Interfaces;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.DeleteSaleInvoice;

public class DeleteSaleInvoiceCommandHandler : IRequestHandler<DeleteSaleInvoiceCommand, bool> {
    private readonly ISaleInvoiceRepository _repository;
    private readonly IStockMovementRepository _stockRepository;
    private readonly ICConnection _connection;

    public DeleteSaleInvoiceCommandHandler(
        ISaleInvoiceRepository repository,
        IStockMovementRepository stockRepository,
        ICConnection connection) {
        _repository = repository;
        _stockRepository = stockRepository;
        _connection = connection;

    }

    public async Task<bool> Handle(DeleteSaleInvoiceCommand request, CancellationToken cancellationToken) {
        var invoice = await _repository.GetByIdAsync(request.Id);
        if (invoice == null) return false;

        await _connection.BeginTransaction();
        try {
            foreach (var item in invoice.Items) {
                await _stockRepository.CreateReversalAsync(
                    item.ProductId,
                    request.Id,
                    item.Quantity,
                    $"Anulación de la factura #{request.Id}");
            }

            var deleted = await _repository.DeleteAsync(request.Id);
            await _connection.CommitTransaction();
            return deleted;
        } catch {
            await _connection.CancelTransaction();
            throw;
        }
    }
}
