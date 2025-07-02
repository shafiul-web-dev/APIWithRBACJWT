namespace APIWithRBACJWT.DTO
{
	namespace APIWithRBACJWT.DTO
	{
		public class UserProfileDto
		{
			public string Id { get; set; } = string.Empty;
			public string FullName { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
			public string PhoneNumber { get; set; } = string.Empty;
			public List<string> Roles { get; set; } = new();
		}
	}
}
