using MediatR;

namespace API.Application.Products.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest<bool>;
