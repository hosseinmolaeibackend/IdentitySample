namespace IdentitySample.Models.ViewModel
{
	public class AddUserToViewModel
	{
		public string UserId { get; set; }
		public List<UserRolesViewModel> UserRoles { get; set; } = new();
	}

	public class UserRolesViewModel
	{
		public string RoleName { get; set; }
		public bool IsSelected { get; set; }
	}
}
