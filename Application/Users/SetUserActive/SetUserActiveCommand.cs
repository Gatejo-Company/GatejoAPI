using MediatR;

namespace API.Application.Users.SetUserActive;

public record SetUserActiveCommand(int Id, bool Active) : IRequest<bool>;
