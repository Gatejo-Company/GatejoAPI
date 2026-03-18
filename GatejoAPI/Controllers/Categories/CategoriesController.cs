using API.Application.Categories.CreateCategory;
using API.Application.Categories.DeleteCategory;
using API.Application.Categories.GetCategories;
using API.Application.Categories.GetCategoryById;
using API.Application.Categories.UpdateCategory;
using API.Application.Categories;
using API.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.API_Clean_Architecture.Controllers.Categories;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<PagedResult<CategoryDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return await _mediator.Send(new GetCategoriesQuery(page, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<CategoryDto> GetById(int id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery(id));
        if (category is null) throw new KeyNotFoundException();
        return category;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<CategoryDto> Create([FromBody] CategoryRequest request)
    {
        return await _mediator.Send(new CreateCategoryCommand(request.Name, request.Description));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<CategoryDto> Update(int id, [FromBody] CategoryRequest request)
    {
        var category = await _mediator.Send(new UpdateCategoryCommand(id, request.Name, request.Description));
        if (category is null) throw new KeyNotFoundException();
        return category;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task Delete(int id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(id));
        if (!result) throw new KeyNotFoundException();
    }
}
