using Microsoft.AspNetCore.Identity;

namespace _userManager
{
	internal class CreateAsync
	{
		private IdentityUser user;
		private string password;

		public CreateAsync(IdentityUser user, string password)
		{
			this.user = user;
			this.password = password;
		}
	}
}