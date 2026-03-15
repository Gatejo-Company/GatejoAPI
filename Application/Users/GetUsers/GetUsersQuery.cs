using API.Application.Shared;
using MediatR;

namespace API.Application.Users.GetUsers;

public record GetUsersQuery(int Page, int PageSize) : IRequest<PagedResult<UserDto>>, IPagedQuery;
