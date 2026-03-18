using API.Application.Categories;
using API.Domain.Categories;
using API.Persistence.Categories.Interfaces;
using MediatR;

namespace API.Application.Categories.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto?>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto?> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.UpdateAsync(request.Id, request.Name, request.Description);
        return category == null ? null : CategoryDto.From(category);
    }
}
