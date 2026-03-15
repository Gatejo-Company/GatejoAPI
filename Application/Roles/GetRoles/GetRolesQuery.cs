using API.Application.Shared;
using MediatR;

namespace API.Application.Roles.GetRoles;

public record GetRolesQuery(int Page, int PageSize) : IRequest<PagedResult<RoleDto>>, IPagedQuery;
