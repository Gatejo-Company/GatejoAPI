using API.Application.Brands;
using MediatR;

namespace API.Application.Brands.GetBrandById;

public record GetBrandByIdQuery(int Id) : IRequest<BrandDto?>;
