using API.Application.Brands;
using MediatR;

namespace API.Application.Brands.UpdateBrand;

public record UpdateBrandCommand(int Id, string Name) : IRequest<BrandDto?>;
