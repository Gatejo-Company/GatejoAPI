using API.Application.Categories;
using MediatR;

namespace API.Application.Categories.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<CategoryDto>;
