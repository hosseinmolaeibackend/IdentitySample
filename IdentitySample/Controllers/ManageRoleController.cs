using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentitySample.Controllers
{
	public class ManageRoleController : Controller
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		public ManageRoleController(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}
		public IActionResult Index()
		{
			var roles = _roleManager.Roles.ToList();
			return View(roles);
		}
		[HttpGet]
		public IActionResult AddRole()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddRole(string Name)
		{
			if (string.IsNullOrEmpty(Name)) return Json("name is null");
			var role = new IdentityRole(Name);
			var result = await _roleManager.CreateAsync(role);
			if (result.Succeeded) return RedirectToAction("Index");
			foreach (var erro in result.Errors)
			{
				ModelState.AddModelError("", erro.Description);
			}
			return View(role);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteRole(string id)
		{
			if (string.IsNullOrEmpty(id)) return Json("id is null");
			var role = await _roleManager.FindByIdAsync(id);
			if (role == null) return Json("role is null");
			await _roleManager.DeleteAsync(role);

			return RedirectToAction("index");
		}

		[HttpGet]
		public async Task<IActionResult> EditRole(string id)
		{
			if (string.IsNullOrEmpty(id)) return Json("id is null");
			var role = await _roleManager.FindByIdAsync(id);
			if (role == null) return Json("role is null");

			return View(role);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditRole(string id, string Name)
		{
			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(Name)) return Json("id or name is null");
			var role = await _roleManager.FindByIdAsync(id);
			if (role == null) return Json("role is empty");
			role.Name = Name;
			var result = await _roleManager.UpdateAsync(role);
			if (result.Succeeded) return RedirectToAction("Index");
			foreach (var erro in result.Errors)
			{
				ModelState.AddModelError("", erro.Description);
			}
			return View(role);
		}
	}
}
