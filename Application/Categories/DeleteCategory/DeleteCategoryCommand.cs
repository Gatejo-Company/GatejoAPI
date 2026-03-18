using MediatR;

namespace API.Application.Categories.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest<bool>;
