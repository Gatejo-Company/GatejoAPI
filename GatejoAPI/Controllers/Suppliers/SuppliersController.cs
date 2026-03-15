using API.Application.Suppliers.CreateSupplier;
using API.Application.Suppliers.DeleteSupplier;
using API.Application.Suppliers.GetSuppliers;
using API.Application.Suppliers.GetSupplierById;
using API.Application.Suppliers.UpdateSupplier;
using API.Application.Suppliers;
using API.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.API_Clean_Architecture.Controllers.Suppliers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<PagedResult<SupplierDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return await _mediator.Send(new GetSuppliersQuery(page, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<SupplierDto> GetById(int id)
    {
        var supplier = await _mediator.Send(new GetSupplierByIdQuery(id));
        if (supplier is null) throw new KeyNotFoundException();
        return supplier;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<SupplierDto> Create([FromBody] SupplierRequest request)
    {
        return await _mediator.Send(new CreateSupplierCommand(request.Name, request.Phone, request.Email, request.Notes));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<SupplierDto> Update(int id, [FromBody] SupplierRequest request)
    {
        var supplier = await _mediator.Send(new UpdateSupplierCommand(id, request.Name, request.Phone, request.Email, request.Notes));
        if (supplier is null) throw new KeyNotFoundException();
        return supplier;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task Delete(int id)
    {
        var result = await _mediator.Send(new DeleteSupplierCommand(id));
        if (!result) throw new KeyNotFoundException();
    }
}
