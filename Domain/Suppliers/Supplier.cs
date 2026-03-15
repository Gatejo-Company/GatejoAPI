namespace API.Domain.Suppliers;

public class Supplier {
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Phone { get; set; }
	public string? Email { get; set; }
	public string? Notes { get; set; }
}
