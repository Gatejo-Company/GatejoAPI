using MediatR;

namespace API.Application.Users.UpdateUser;

public record UpdateUserCommand(int Id, string Name, string Email, int RoleId) : IRequest<UserDto?>;
