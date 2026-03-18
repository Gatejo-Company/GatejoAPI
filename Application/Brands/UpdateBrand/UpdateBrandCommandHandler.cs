using API.Application.Brands;
using API.Domain.Brands;
using API.Persistence.Brands.Interfaces;
using MediatR;

namespace API.Application.Brands.UpdateBrand;

public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, BrandDto?>
{
    private readonly IBrandRepository _brandRepository;

    public UpdateBrandCommandHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<BrandDto?> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.UpdateAsync(request.Id, request.Name);
        return brand == null ? null : BrandDto.From(brand);
    }
}
