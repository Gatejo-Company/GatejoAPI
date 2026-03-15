using API.Application.Brands;
using API.Application.Shared;
using API.Domain.Brands;
using API.Persistence.Brands.Interfaces;
using API.Persistence.Shared;
using MediatR;

namespace API.Application.Brands.GetBrands;

public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, PagedResult<BrandDto>>
{
    private readonly IBrandRepository _brandRepository;

    public GetBrandsQueryHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<PagedResult<BrandDto>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        var filter = new PagedFilter(request.Page, request.PageSize);
        var data = await _brandRepository.GetAllAsync(filter);
        return data.ToPagedResult(filter.PageSize, BrandDto.From);
    }
}
