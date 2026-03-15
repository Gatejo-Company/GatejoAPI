using API.Application.Brands;
using MediatR;

namespace API.Application.Brands.CreateBrand;

public record CreateBrandCommand(string Name) : IRequest<BrandDto>;
