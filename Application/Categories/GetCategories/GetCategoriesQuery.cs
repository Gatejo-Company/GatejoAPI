using API.Application.Categories;
using API.Application.Shared;
using MediatR;

namespace API.Application.Categories.GetCategories;

public record GetCategoriesQuery(int Page, int PageSize) : IRequest<PagedResult<CategoryDto>>, IPagedQuery;
