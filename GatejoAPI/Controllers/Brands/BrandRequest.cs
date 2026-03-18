using System.ComponentModel.DataAnnotations;

namespace API.API_Clean_Architecture.Controllers.Brands;

public class BrandRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
