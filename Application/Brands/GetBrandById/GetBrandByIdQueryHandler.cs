using API.Application.Brands;
using API.Domain.Brands;
using API.Persistence.Brands.Interfaces;
using MediatR;

namespace API.Application.Brands.GetBrandById;

public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, BrandDto?>
{
    private readonly IBrandRepository _brandRepository;

    public GetBrandByIdQueryHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<BrandDto?> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(request.Id);
        return brand == null ? null : BrandDto.From(brand);
    }
}
