using API.Domain.Categories;

namespace API.Application.Categories;

public record CategoryDto(int Id, string Name, string? Description) {
	public static CategoryDto From(Category category) => new(category.Id, category.Name, category.Description);
}
