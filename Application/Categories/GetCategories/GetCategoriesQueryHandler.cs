using API.Application.Categories;
using API.Application.Shared;
using API.Domain.Categories;
using API.Persistence.Categories.Interfaces;
using API.Persistence.Shared;
using MediatR;

namespace API.Application.Categories.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var filter = new PagedFilter(request.Page, request.PageSize);
        var data = await _categoryRepository.GetAllAsync(filter);
        return data.ToPagedResult(filter.PageSize, CategoryDto.From);
    }
}
