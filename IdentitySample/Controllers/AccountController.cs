using IdentitySample.Models.ViewModel;
using IdentitySample.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentitySample.Controllers
{
	public class AccountController(
		UserManager<IdentityUser> _userManager,
		SignInManager<IdentityUser> _signInManager,
		IMessageSender _massegeSender) : Controller
	{

		#region Register
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
		{
			if (ModelState.IsValid)
			{
				var user = new IdentityUser
				{
					UserName = registerViewModel.UserName,
					Email = registerViewModel.Email
				};
				var result = await _userManager.CreateAsync(user, registerViewModel.Password);



				if (result.Succeeded)
				{
					var emailConfrimationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					var emailMassege =
						Url.Action("ConfrimEmail", "Account", new
						{
							username = user.UserName,
							token = emailConfrimationToken
						}, Request.Scheme);

					await _massegeSender.SendEmailAsync(registerViewModel.Email, "Email confrimtion", emailMassege);


					return RedirectToAction("Index", "Home");
				}
				else if (!result.Succeeded)
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
				}
			}
			return View(registerViewModel);
		}

		[HttpGet]
		public async Task<IActionResult> ConfrimEmail(string userName, string token)
		{
			if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(token))
				return NotFound();
			var user = await _userManager.FindByNameAsync(userName);
			if (user == null) return NotFound();
			var result = await _userManager.ConfirmEmailAsync(user, token);

			return Content(result.Succeeded ? "Email Confirmed" : "Email not Confirmed");
		}
		#endregion


		#region Login
		[HttpGet]
		public IActionResult Login(string? returnurl)
		{
			ViewData["returnurl"] = returnurl;
			return _signInManager.IsSignedIn(User) ? RedirectToAction("Index", "Home") : View();
			//if (_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
			//return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model, string? returnurl)
		{
			if (_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");

			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.Remeberme, true);

				//return result.Succeeded ? RedirectToAction("Index", "Home") : result.IsLockedOut ? Json("your account 5 minutes lock") :RedirectToAction("Login");
				if (result.Succeeded)
				{
					if (!string.IsNullOrEmpty(returnurl) && Url.IsLocalUrl(returnurl))
						return Redirect(returnurl);
					return RedirectToAction("Index", "Home");
				}
				if (result.IsLockedOut)
				{
					return Json("your account 5 minutes lock");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
				}
			}
			return View(model);
		}
		#endregion

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> IsEmailInUse(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			return user == null
				? Json(true)
				: Json($"ایمیل {email} از قبل ثبت شده است");
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> IsUserNameInUse(string username)
		{
			var user = await _userManager.FindByNameAsync(username);
			return user == null
				? Json(true)
				: Json($"نام کاربری {username} از قبل ثبت شده است");
		}
	
	}
}
