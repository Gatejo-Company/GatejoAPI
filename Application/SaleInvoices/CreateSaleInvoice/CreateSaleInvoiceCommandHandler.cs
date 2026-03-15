using API.Application.SaleInvoices;
using API.Domain.StockMovements;
using API.Persistence.SaleInvoices.Interfaces;
using API.Persistence.Shared;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.CreateSaleInvoice;

public class CreateSaleInvoiceCommandHandler : IRequestHandler<CreateSaleInvoiceCommand, SaleInvoiceDto> {
	private readonly ISaleInvoiceRepository _repository;
	private readonly IStockMovementRepository _stockRepository;
	private readonly ITransactionManager _tx;

	public CreateSaleInvoiceCommandHandler(
		ISaleInvoiceRepository repository,
		IStockMovementRepository stockRepository,
		ITransactionManager tx) {
		_repository = repository;
		_stockRepository = stockRepository;
		_tx = tx;
	}

	public async Task<SaleInvoiceDto> Handle(CreateSaleInvoiceCommand request, CancellationToken cancellationToken) {
        bool isReversalInvoice = request.Reversed;

        var total = request.Items.Sum(i => i.Quantity * i.UnitPrice) * (isReversalInvoice ? -1 : 1);

		await _tx.BeginAsync();
		try {
			var invoiceId = await _repository.CreateAsync(request.Date, request.OnCredit, false, total, request.Notes);

			foreach (var item in request.Items) {
                var subtotal = (item.Quantity * item.UnitPrice) * (isReversalInvoice ? -1 : 1);

				int quantity = item.Quantity * (isReversalInvoice ? -1 : 1);
                decimal unitPrice = item.UnitPrice * (isReversalInvoice ? -1 : 1);

                await _repository.AddItemAsync(invoiceId, item.ProductId, quantity, unitPrice, subtotal);

				string note = (isReversalInvoice ? "Reversal of sale invoice" : "Sale invoice") + $" #{invoiceId}";

				await _stockRepository.CreateForInvoiceAsync(
					MovementTypeNames.Sale,
					item.ProductId,
					item.Quantity * (isReversalInvoice? 1 : -1),
					invoiceId,
                    note);

			}

			if (isReversalInvoice && request.IdReverdesInvoice > 0) {
				await _repository.ReverseAsync(request.IdReverdesInvoice);
            }

			await _tx.CommitAsync();
			return SaleInvoiceDto.From((await _repository.GetByIdAsync(invoiceId))!);
		} catch {
			await _tx.RollbackAsync();
			throw;
		}
	}
}
