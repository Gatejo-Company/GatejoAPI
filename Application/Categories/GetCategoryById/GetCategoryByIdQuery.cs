using API.Application.Categories;
using MediatR;

namespace API.Application.Categories.GetCategoryById;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto?>;
