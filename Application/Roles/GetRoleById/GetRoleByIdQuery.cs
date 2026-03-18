using MediatR;

namespace API.Application.Roles.GetRoleById;

public record GetRoleByIdQuery(int Id) : IRequest<RoleDto?>;
