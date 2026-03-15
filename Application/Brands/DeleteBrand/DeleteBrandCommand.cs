using MediatR;

namespace API.Application.Brands.DeleteBrand;

public record DeleteBrandCommand(int Id) : IRequest<bool>;
