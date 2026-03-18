namespace API.Persistence.StockMovements.Interfaces;

public record StockMovementsFilter(int? ProductId, int? TypeId, DateTime? From, DateTime? To, int Page, int PageSize);
