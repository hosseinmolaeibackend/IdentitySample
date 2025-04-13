using System.ComponentModel.DataAnnotations;

namespace IdentitySample.Models.ViewModel
{
	public class LoginViewModel
	{
		[Required(ErrorMessage ="Email Address is empty")]
		[Display(Name ="Username")]
		public string Username { get; set; }
		[Required(ErrorMessage = "password is empty")]
		[Display(Name = "Password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[Display(Name = "Remeber me")]
		public bool Remeberme { get; set; }

	}
}
