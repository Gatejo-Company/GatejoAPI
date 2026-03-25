using API.DataAccess.Interfaces;
using API.Domain.StockMovements;
using API.Persistence.SaleInvoices.Interfaces;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.CreateSaleInvoice;

public class CreateSaleInvoiceCommandHandler : IRequestHandler<CreateSaleInvoiceCommand, SaleInvoiceDto> {
    private readonly ISaleInvoiceRepository _repository;
    private readonly IStockMovementRepository _stockRepository;
    private readonly ICConnection _connection;

    public CreateSaleInvoiceCommandHandler(
        ISaleInvoiceRepository repository,
        IStockMovementRepository stockRepository,
        ICConnection connection) {
        _repository = repository;
        _stockRepository = stockRepository;
        _connection = connection;
    }

    public async Task<SaleInvoiceDto> Handle(CreateSaleInvoiceCommand request, CancellationToken cancellationToken) {
        var total = request.Items.Sum(i => i.Amount);

        await _connection.BeginTransaction();
        try {
            var invoiceId = await _repository.CreateAsync(request.Date, request.OnCredit, false, total, request.Notes);

            foreach (var item in request.Items) {
                await _repository.AddItemAsync(invoiceId, item.ProductId, item.Quantity, item.UnitPrice, item.Amount);

                string note = $"Factura #{invoiceId}";

                await _stockRepository.CreateForInvoiceAsync(
                    MovementTypeNames.Sale,
                    item.ProductId,
                    item.Quantity * -1,
                    invoiceId,
                    note);

            }

            await _connection.CommitTransaction();
            return SaleInvoiceDto.From((await _repository.GetByIdAsync(invoiceId))!);
        } catch {
            await _connection.CancelTransaction();
            throw;
        }
    }
}
