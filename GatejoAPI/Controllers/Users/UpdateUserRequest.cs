using System.ComponentModel.DataAnnotations;

namespace API.API_Clean_Architecture.Controllers.Users;

public class UpdateUserRequest {
	[Required, MaxLength(100)]
	public string Name { get; set; } = string.Empty;

	[Required, EmailAddress, MaxLength(100)]
	public string Email { get; set; } = string.Empty;

	[Required]
	public int RoleId { get; set; }
}
