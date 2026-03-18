using API.Domain.Users;

namespace API.Application.Users;

public record UserDto(int Id, string Name, string Email, int RoleId, string RoleName, bool Active, DateTime CreatedAt) {
	public static UserDto From(User user) => new(user.Id, user.Name, user.Email, user.RoleId, user.RoleName, user.Active, user.CreatedAt);
}
