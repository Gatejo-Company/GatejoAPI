using API.Application.Products;
using MediatR;

namespace API.Application.Products.UpdateProductPrice;

public record UpdateProductPriceCommand(int Id, decimal Price, string? Reason) : IRequest<ProductDto?>;
