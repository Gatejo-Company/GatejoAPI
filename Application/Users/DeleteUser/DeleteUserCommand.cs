using MediatR;

namespace API.Application.Users.DeleteUser;

public record DeleteUserCommand(int Id) : IRequest<bool>;
