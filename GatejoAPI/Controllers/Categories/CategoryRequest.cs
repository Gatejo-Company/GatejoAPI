using System.ComponentModel.DataAnnotations;

namespace API.API_Clean_Architecture.Controllers.Categories;

public class CategoryRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
