using IdentitySample.Models.ViewModel;
using IdentitySample.Models.ViewModel.RoleManager;
using IdentitySample.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentitySample.Controllers
{
	public class ManageUserController : Controller
	{
		#region Dependency Injection
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		SignInManager<IdentityUser> _signInManager;
		public ManageUserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
			SignInManager<IdentityUser> signInManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			_signInManager = signInManager;
		}
		#endregion

		#region Main page
		public IActionResult Index()
		{
			var model = _userManager.Users.Select(a => new IndexViewModel()
			{
				Id = a.Id,
				UserName = a.UserName,
				Email = a.Email,
			}).ToList();
			return View(model);
		}
		#endregion

		#region Edit User
		[HttpGet]
		public async Task<IActionResult> EditUser(string id)
		{
			if (string.IsNullOrEmpty(id)) return Json("id is null");
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return Json("user is not find");
			return View(user);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditUser(string id, string userName)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(id);
				if (user == null)
				{
					return Json("user is not find");
				}
				user.UserName = userName;
				var result = await _userManager.UpdateAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction("Index", "ManageUser");
				}
				else
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}
				}
			}
			ViewData["error"] = "model state is null";
			return View();
		}
		#endregion

		#region Add Role To User

		[HttpGet]
		public async Task<IActionResult> AddRoleToUser(string id)
		{
			if (string.IsNullOrEmpty(id)) return Json("id is null");
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return Json("user is not find");

			var roles = _roleManager.Roles.AsTracking().
				Select(r => r.Name).ToList();
			var userRoles = await _userManager.GetRolesAsync(user);
			var validRoles = roles.Where(r => (!userRoles.Contains(r)))
				.Select(x => new UserRolesViewModel() { RoleName = x }).ToList();

			var model = new AddUserToViewModel() { UserId = id, UserRoles = validRoles };
			//var model = new AddUserToViewModel()
			//{
			//	UserId = id,
			//};

			//foreach (var role in roles)
			//{
			//	if (!await _userManager.IsInRoleAsync(user, role.Name))
			//	{
			//		model.UserRoles.Add(new UserRolesViewModel()
			//		{
			//			RoleName = role.Name
			//		});
			//	}
			//}

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> AddRoleToUser(AddUserToViewModel model)
		{
			if (model == null) return Json("model is null");


			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null) return Json("user is not find");
			var requestRoles = model.UserRoles.Where(a => a.IsSelected).Select(a => a.RoleName).ToList();

			var result = await _userManager.AddToRolesAsync(user, requestRoles);

			if (result.Succeeded)
			{
				await _signInManager.RefreshSignInAsync(user);
				return RedirectToAction("Index", "ManageUser");
			}
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			return View(model);
		}
		#endregion

		#region Remove Role From User

		[HttpGet]
		public async Task<IActionResult> RemoveRoleFromUser(string id)
		{
			if (string.IsNullOrEmpty(id)) return Json("Id is null");
			var user = await _userManager.FindByIdAsync(id);

			if (user == null) return Json("user is null");

			var roles = await _roleManager.Roles.AsTracking().Select(r => r.Name).ToListAsync();
			var userRoles = await _userManager.GetRolesAsync(user);
			var validRoles = userRoles.Select(r => new UserRolesViewModel() { RoleName = r }).ToList();
			var model = new AddUserToViewModel() { UserId = id, UserRoles = validRoles };

			//foreach (var role in roles)
			//{
			//	if (await _userManager.IsInRoleAsync(user, role.Name))
			//	{
			//		model.UserRoles.Add(new UserRolesViewModel
			//		{
			//			RoleName = role.Name
			//		});
			//	}
			//}
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> RemoveRoleFromUser(AddUserToViewModel model)
		{
			if (model == null) return Json("model is null");
			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null) return Json("user is null");
			var requestRoles = model.UserRoles.Where(x => x.IsSelected)
				.Select(x => x.RoleName).ToList();
			var result = await _userManager.RemoveFromRolesAsync(user, requestRoles);
			if (result.Succeeded)
			{
				await _signInManager.RefreshSignInAsync(user);
				return RedirectToAction("Index", "ManageUser");
			}
			foreach (var role in result.Errors)
			{
				ModelState.AddModelError("", role.Description);
			}
			return View(model);
		}
		#endregion

		#region Delete User
		[HttpPost]
		public async Task<IActionResult> DeleteUser(string userId)
		{
			if (userId == null) return Json("userId is null");
			var useExist = await _userManager.FindByIdAsync(userId);
			if (useExist == null) return Json("user is null");
			await _userManager.DeleteAsync(useExist);
			return RedirectToAction("Index", "ManageUser");
		}
		#endregion

		#region Add Claim
		[HttpGet]
		public async Task<IActionResult> AddUserToClaim(string id)
		{
			if (string.IsNullOrEmpty(id)) return Json("id is null");
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return Json("user is not find");
			var allclaims = ClaimStore.AllClaims;
			var userClaim = await _userManager.GetClaimsAsync(user);
			var validClaim = allclaims.Where(e => userClaim.All(x => x.Type != e.Type))
				.Select(f => new ClaimsViewModel(f.Type)).ToList();
			var model
				= new AddOrRemoveClaimViewModel(id, validClaim);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddUserToClaim(AddOrRemoveClaimViewModel model)
		{
			if (model == null) return Json("model is null");
			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null) return Json("user is not find");
			var requestClaim = model.UserClaims.Where(r => r.IsSelected)
				.Select(r => new Claim(r.ClaimType, true.ToString())).ToList();
			var result = await
				_userManager.AddClaimsAsync(user, requestClaim);

			if (result.Succeeded)
			{
				await _signInManager.RefreshSignInAsync(user);
				return RedirectToAction("Index");
			}
			foreach (var claim in result.Errors)
			{
				ModelState.AddModelError("", claim.Description);
			}
			return View(model);
		}

		#endregion

		#region Remove Claim
		[HttpGet]
		public async Task<IActionResult> RemoveUserToClaim(string id)
		{
			if (string.IsNullOrEmpty(id)) return Json("id is null");
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return Json("user is not find");

			var userClaim = await _userManager.GetClaimsAsync(user);
			var validModel = userClaim.Select(x => new ClaimsViewModel(x.Type)).ToList();
			var model = new AddOrRemoveClaimViewModel(id, validModel);

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemoveUserToClaim(AddOrRemoveClaimViewModel model)
		{
			if (model == null) return Json("model is null");

			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null) return Json("user is not find");

			var requestClaims = model.UserClaims.Where(r => r.IsSelected)
				.Select(u => new Claim(u.ClaimType, true.ToString())).ToList();

			var result = await _userManager.RemoveClaimsAsync(user, requestClaims);
			if (result.Succeeded)
			{
				await _signInManager.RefreshSignInAsync(user);
				return RedirectToAction("Index");
			}

			foreach (var claim in result.Errors)
			{
				ModelState.AddModelError("", claim.Description);
			}
			return View(model);
		}
		#endregion
	}
}
