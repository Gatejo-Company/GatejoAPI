using API.Application.Brands.CreateBrand;
using API.Application.Brands.DeleteBrand;
using API.Application.Brands.GetBrands;
using API.Application.Brands.GetBrandById;
using API.Application.Brands.UpdateBrand;
using API.Application.Brands;
using API.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.API_Clean_Architecture.Controllers.Brands;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BrandsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<PagedResult<BrandDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return await _mediator.Send(new GetBrandsQuery(page, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<BrandDto> GetById(int id)
    {
        var brand = await _mediator.Send(new GetBrandByIdQuery(id));
        if (brand is null) throw new KeyNotFoundException();
        return brand;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<BrandDto> Create([FromBody] BrandRequest request)
    {
        return await _mediator.Send(new CreateBrandCommand(request.Name));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<BrandDto> Update(int id, [FromBody] BrandRequest request)
    {
        var brand = await _mediator.Send(new UpdateBrandCommand(id, request.Name));
        if (brand is null) throw new KeyNotFoundException();
        return brand;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task Delete(int id)
    {
        var result = await _mediator.Send(new DeleteBrandCommand(id));
        if (!result) throw new KeyNotFoundException();
    }
}
