using API.Application.Categories;
using MediatR;

namespace API.Application.Categories.UpdateCategory;

public record UpdateCategoryCommand(int Id, string Name, string? Description) : IRequest<CategoryDto?>;
