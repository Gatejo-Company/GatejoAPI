using API.Domain.StockMovements;
using API.Persistence.Shared;

namespace API.Persistence.StockMovements.Interfaces;

public interface IStockMovementRepository {
	Task<PagedData<StockMovement>> GetAllAsync(StockMovementsFilter filter);
	Task<StockMovement?> GetByIdAsync(int id);
	Task<StockMovement> CreateManualAdjustmentAsync(int productId, int typeId, int quantity, string? notes);
	Task<List<StockMovement>> GetByProductAsync(int productId);
	Task CreateForInvoiceAsync(string movementType, int productId, int quantity, int referenceId, string notes);
	Task CreateReversalAsync(int productId, int referenceId, int quantityDelta, string notes);
}
