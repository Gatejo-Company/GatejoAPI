using API.Application.Products;
using MediatR;

namespace API.Application.Products.CreateProduct;

public record CreateProductCommand(
	string Name,
	string? Description,
	int CategoryId,
	int BrandId,
	decimal Price,
	int MinStock
) : IRequest<ProductDto>;
