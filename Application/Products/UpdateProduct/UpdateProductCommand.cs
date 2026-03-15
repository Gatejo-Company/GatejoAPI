using API.Application.Products;
using MediatR;

namespace API.Application.Products.UpdateProduct;

public record UpdateProductCommand(int Id, string Name, string? Description, int CategoryId, int BrandId, int MinStock) : IRequest<ProductDto?>;
