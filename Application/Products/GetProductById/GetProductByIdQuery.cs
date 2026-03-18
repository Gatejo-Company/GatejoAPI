using API.Application.Products;
using MediatR;

namespace API.Application.Products.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
