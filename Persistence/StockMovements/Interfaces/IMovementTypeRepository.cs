using API.Domain.StockMovements;
using API.Persistence.Shared;

namespace API.Persistence.StockMovements.Interfaces;

public interface IMovementTypeRepository {
	Task<PagedData<MovementType>> GetAllAsync(PagedFilter filter);
	Task<MovementType?> GetByIdAsync(int id);
}
