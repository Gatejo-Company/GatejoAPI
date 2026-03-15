using System.ComponentModel.DataAnnotations;

namespace API.API_Clean_Architecture.Controllers.Suppliers;

public class SupplierRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Phone { get; set; }

    [EmailAddress, MaxLength(100)]
    public string? Email { get; set; }

    public string? Notes { get; set; }
}
