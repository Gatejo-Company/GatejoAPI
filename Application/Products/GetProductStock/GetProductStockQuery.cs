using MediatR;

namespace API.Application.Products.GetProductStock;

public record GetProductStockQuery(int ProductId) : IRequest<int>;
