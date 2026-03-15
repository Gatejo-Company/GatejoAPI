using API.Application.Categories;
using API.Domain.Categories;
using API.Persistence.Categories.Interfaces;
using MediatR;

namespace API.Application.Categories.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.CreateAsync(request.Name, request.Description);
        return CategoryDto.From(category);
    }
}
