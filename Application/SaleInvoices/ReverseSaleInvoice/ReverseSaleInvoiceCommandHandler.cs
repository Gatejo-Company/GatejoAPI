using API.Application.SaleInvoices;
using API.Domain.StockMovements;
using API.Persistence.SaleInvoices.Interfaces;
using API.Persistence.Shared;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.ReverseSaleInvoice;

public class ReverseSaleInvoiceCommandHandler : IRequestHandler<ReverseSaleInvoiceCommand, SaleInvoiceDto> {
	private readonly ISaleInvoiceRepository _repository;
	private readonly IStockMovementRepository _stockRepository;
	private readonly ITransactionManager _tx;

	public ReverseSaleInvoiceCommandHandler(
		ISaleInvoiceRepository repository,
		IStockMovementRepository stockRepository,
		ITransactionManager tx) {
		_repository = repository;
		_stockRepository = stockRepository;
		_tx = tx;
	}

	public async Task<SaleInvoiceDto> Handle(ReverseSaleInvoiceCommand request, CancellationToken cancellationToken) {
		var original = await _repository.GetByIdAsync(request.Id);

		if (original == null)
			throw new KeyNotFoundException($"Sale invoice #{request.Id} not found.");

		if (original.Reversed)
			throw new InvalidOperationException($"Sale invoice #{request.Id} has already been reversed.");

		var newTotal = -original.Total;

		await _tx.BeginAsync();
		try {
			var newInvoiceId = await _repository.CreateAsync(original.Date, original.OnCredit, false, newTotal, original.Notes);

			foreach (var item in original.Items) {
				await _repository.AddItemAsync(newInvoiceId, item.ProductId, -item.Quantity, -item.UnitPrice, -item.Subtotal);

				string note = $"Reversal of sale invoice #{request.Id} → new invoice #{newInvoiceId}";

				// stockDelta = item.Quantity (already signed): undoes the original stock movement
				await _stockRepository.CreateForInvoiceAsync(
					MovementTypeNames.Sale,
					item.ProductId,
					item.Quantity,
					newInvoiceId,
					note);
			}

			await _repository.ReverseAsync(request.Id);

			await _tx.CommitAsync();
			return SaleInvoiceDto.From((await _repository.GetByIdAsync(newInvoiceId))!);
		} catch {
			await _tx.RollbackAsync();
			throw;
		}
	}
}
