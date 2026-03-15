using MediatR;

namespace API.Application.Users.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<UserDto?>;
