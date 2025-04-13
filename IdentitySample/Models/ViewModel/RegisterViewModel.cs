using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentitySample.Models.ViewModel
{
	public class RegisterViewModel
	{
		[Display(Name = "User Name")]
		[Required(ErrorMessage = "user name is empty")]
		[Remote("IsUserNameInUse", "Account", HttpMethod = "POST", AdditionalFields = "_RequestVerficationToken")]
		public string UserName { get; set; } = default!;

		[Display(Name = "Email")]
		[Required(ErrorMessage = "email is empty")]
		[DataType(DataType.EmailAddress)]
		[Remote("IsEmailInUse", "Account", HttpMethod = "POST", AdditionalFields = "_RequestVerficationToken")]
		public string Email { get; set; } = default!;

		[Display(Name = "password")]
		[Required(ErrorMessage = "password is empty")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = default!;

		[Display(Name = "confrim password")]
		[Required(ErrorMessage = "confrimpassword is empty")]
		[DataType(DataType.Password)]
		[Compare(nameof(Password))]
		public string ConfrimPassword { get; set; } = default!;

	}
}
