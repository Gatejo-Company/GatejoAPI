using MediatR;

namespace API.Application.Products.SetProductActive;

public record SetProductActiveCommand(int Id, bool Active) : IRequest<bool>;
