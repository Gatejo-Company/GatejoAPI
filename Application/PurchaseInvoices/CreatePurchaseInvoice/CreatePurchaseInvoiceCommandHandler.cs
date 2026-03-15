using API.DataAccess.Interfaces;
using API.Domain.StockMovements;
using API.Persistence.PurchaseInvoices.Interfaces;
using API.Persistence.Shared;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.PurchaseInvoices.CreatePurchaseInvoice;

public class CreatePurchaseInvoiceCommandHandler : IRequestHandler<CreatePurchaseInvoiceCommand, PurchaseInvoiceDto> {
    private readonly IPurchaseInvoiceRepository _repository;
    private readonly IStockMovementRepository _stockRepository;
    private readonly ITransactionManager _tx;
    private readonly ICConnection _connection;

    public CreatePurchaseInvoiceCommandHandler(
        IPurchaseInvoiceRepository repository,
        IStockMovementRepository stockRepository,
        ITransactionManager tx,
        ICConnection connection) {
        _repository = repository;
        _stockRepository = stockRepository;
        _tx = tx;
        _connection = connection;
    }

    public async Task<PurchaseInvoiceDto> Handle(CreatePurchaseInvoiceCommand request, CancellationToken cancellationToken) {
        var total = request.Items.Sum(i => i.Quantity * i.UnitPrice);
        await _connection.Connect();

        await _tx.BeginAsync();
        try {
            var invoiceId = await _repository.CreateAsync(request.SupplierId, request.Date, total, request.Notes);

            foreach (var item in request.Items) {
                await _repository.AddItemAsync(invoiceId, item.ProductId, item.Quantity, item.UnitPrice);
                await _stockRepository.CreateForInvoiceAsync(
                    MovementTypeNames.Purchase,
                    item.ProductId,
                    item.Quantity,
                    invoiceId,
                    $"Purchase invoice #{invoiceId}");
            }

            await _tx.CommitAsync();
            return PurchaseInvoiceDto.From((await _repository.GetByIdAsync(invoiceId))!);
        } catch {
            await _tx.RollbackAsync();
            throw;
        }
    }
}
