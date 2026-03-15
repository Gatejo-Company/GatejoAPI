using System.ComponentModel.DataAnnotations;

namespace API.API_Clean_Architecture.Controllers.StockMovements;

public class StockMovementRequest {
	[Required]
	public int ProductId { get; set; }
	[Required]
	public int TypeId { get; set; }
	[Required]
	public int Quantity { get; set; }
	public string? Notes { get; set; }
}
