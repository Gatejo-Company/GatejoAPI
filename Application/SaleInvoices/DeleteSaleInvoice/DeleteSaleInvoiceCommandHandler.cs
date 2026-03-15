using API.Persistence.SaleInvoices.Interfaces;
using API.Persistence.Shared;
using API.Persistence.StockMovements.Interfaces;
using MediatR;

namespace API.Application.SaleInvoices.DeleteSaleInvoice;

public class DeleteSaleInvoiceCommandHandler : IRequestHandler<DeleteSaleInvoiceCommand, bool> {
	private readonly ISaleInvoiceRepository _repository;
	private readonly IStockMovementRepository _stockRepository;
	private readonly ITransactionManager _tx;

	public DeleteSaleInvoiceCommandHandler(
		ISaleInvoiceRepository repository,
		IStockMovementRepository stockRepository,
		ITransactionManager tx) {
		_repository = repository;
		_stockRepository = stockRepository;
		_tx = tx;
	}

	public async Task<bool> Handle(DeleteSaleInvoiceCommand request, CancellationToken cancellationToken) {
		var invoice = await _repository.GetByIdAsync(request.Id);
		if (invoice == null) return false;

		await _tx.BeginAsync();
		try {
			foreach (var item in invoice.Items) {
				await _stockRepository.CreateReversalAsync(
					item.ProductId,
					request.Id,
					item.Quantity,
					$"Reversal of sale invoice #{request.Id}");
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
