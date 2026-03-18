using API.Application.Brands;
using API.Domain.Brands;
using API.Persistence.Brands.Interfaces;
using MediatR;

namespace API.Application.Brands.CreateBrand;

public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, BrandDto>
{
    private readonly IBrandRepository _brandRepository;

    public CreateBrandCommandHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<BrandDto> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.CreateAsync(request.Name);
        return BrandDto.From(brand);
    }
}
