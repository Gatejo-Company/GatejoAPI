using API.DataAccess.Interfaces;
using API.Domain.StockMovements;
using API.Persistence.PurchaseInvoices.Interfaces;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.PurchaseInvoices.CreatePurchaseInvoice;

public class CreatePurchaseInvoiceCommandHandler : IRequestHandler<CreatePurchaseInvoiceCommand, PurchaseInvoiceDto> {
    private readonly IPurchaseInvoiceRepository _repository;
    private readonly IStockMovementRepository _stockRepository;

    private readonly ICConnection _connection;

    public CreatePurchaseInvoiceCommandHandler(
        IPurchaseInvoiceRepository repository,
        IStockMovementRepository stockRepository,
        ICConnection connection) {
        _repository = repository;
        _stockRepository = stockRepository;
        _connection = connection;
    }

    public async Task<PurchaseInvoiceDto> Handle(CreatePurchaseInvoiceCommand request, CancellationToken cancellationToken) {
        var total = request.Items.Sum(i => i.Quantity * i.UnitPrice);


        await _connection.BeginTransaction();
        try {
            var invoiceId = await _repository.CreateAsync(request.SupplierId, request.Date, total, request.Notes);

            foreach (var item in request.Items) {
                await _repository.AddItemAsync(invoiceId, item.ProductId, item.Quantity, item.UnitPrice);
                await _stockRepository.CreateForInvoiceAsync(
                    MovementTypeNames.Purchase,
                    item.ProductId,
                    item.Quantity,
                    invoiceId,
                    $"Compra #{invoiceId}");
            }

            await _connection.CommitTransaction();

            var purchase = await _repository.GetByIdAsync(invoiceId);

            return PurchaseInvoiceDto.From(purchase!);
        } catch {
            await _connection.CancelTransaction();
            throw;
        }
    }
}
