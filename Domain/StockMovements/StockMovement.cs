namespace API.Domain.StockMovements;

public class StockMovement {
	public int Id { get; set; }
	public int ProductId { get; set; }
	public string ProductName { get; set; } = string.Empty;
	public int TypeId { get; set; }
	public string TypeName { get; set; } = string.Empty;
	public int Quantity { get; set; }
	public int? ReferenceId { get; set; }
	public string? Notes { get; set; }
	public DateTime CreatedAt { get; set; }
}
