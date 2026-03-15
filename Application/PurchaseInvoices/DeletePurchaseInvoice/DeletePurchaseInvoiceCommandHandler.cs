using API.DataAccess.Interfaces;
using API.Persistence.PurchaseInvoices.Interfaces;
using API.Persistence.Shared;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.PurchaseInvoices.DeletePurchaseInvoice;

public class DeletePurchaseInvoiceCommandHandler : IRequestHandler<DeletePurchaseInvoiceCommand, bool> {
    private readonly IPurchaseInvoiceRepository _repository;
    private readonly IStockMovementRepository _stockRepository;
    private readonly ITransactionManager _tx;
    private readonly ICConnection _connection;

    public DeletePurchaseInvoiceCommandHandler(
        IPurchaseInvoiceRepository repository,
        IStockMovementRepository stockRepository,
        ITransactionManager tx,
        ICConnection connection) {
        _repository = repository;
        _stockRepository = stockRepository;
        _tx = tx;
        _connection = connection;
    }

    public async Task<bool> Handle(DeletePurchaseInvoiceCommand request, CancellationToken cancellationToken) {
        var invoice = await _repository.GetByIdAsync(request.Id);
        if (invoice == null) return false;

        await _connection.Connect();
        await _tx.BeginAsync();
        try {
            foreach (var item in invoice.Items) {
                await _stockRepository.CreateReversalAsync(
                    item.ProductId,
                    request.Id,
                    -item.Quantity,
                    $"Reversal of purchase invoice #{request.Id}");
            }

            var deleted = await _repository.DeleteAsync(request.Id);
            await _tx.CommitAsync();
            return deleted;
        } catch {
            await _tx.RollbackAsync();
            throw;
        }
    }
}
