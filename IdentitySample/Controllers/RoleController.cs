using IdentitySample.Context;
using IdentitySample.Models.ViewModel;
using IdentitySample.Models.ViewModel.Role;
using IdentitySample.Utitlities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitySample.Controllers
{
	public class RoleController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IMemoryCache _memoryCache;
		private readonly IUtilities _utilities;
		public RoleController(ApplicationDbContext context, IUtilities utilities, IMemoryCache memoryCache, RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
			_context = context;
			_memoryCache = memoryCache;
			_utilities = utilities;
		}
		public IActionResult Index()
		{
			var model = _context.SiteSettings.ToList();
			return View(model);
		}

		[HttpGet]
		public IActionResult RoleValidationGuid()
		{
			var roleValidationGuidSiteSetting =
				_context.SiteSettings.FirstOrDefault(c => c.Key == "RoleValidationGuid");

			var model = new RoleValidateGuidViewModel()
			{
				Value = roleValidationGuidSiteSetting?.Value,
				LastChangeTime = roleValidationGuidSiteSetting?.LastTimeChanged
			};
			return View(model);
		}
		[HttpPost]
		public IActionResult RoleValidationGuid(RoleValidateGuidViewModel model)
		{
			var roleValidationGuidSiteSetting =
				_context.SiteSettings.FirstOrDefault(c => c.Key == "RoleValidationGuid");
			if (roleValidationGuidSiteSetting != null)
			{
				_context.SiteSettings.Add(new Models.SiteSetting()
				{
					Key = "RoleValidationGuid",
					Value = Guid.NewGuid().ToString(),
					LastTimeChanged = DateTime.Now
				});
			}
			else
			{
				roleValidationGuidSiteSetting.Value = Guid.NewGuid().ToString();
				roleValidationGuidSiteSetting.LastTimeChanged = DateTime.Now;
				_context.SiteSettings.Update(roleValidationGuidSiteSetting);
			}
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public IActionResult CreateRole()
		{
			var allMvcNames =
				_memoryCache.GetOrCreate("ActionAndControllerNameList", p =>
				{
					p.AbsoluteExpiration = DateTimeOffset.MaxValue;
					return _utilities.ActionAndControllerNameList();
				});
			var model = new CreateRoleViewModel()
			{
				ActionAndControllerNames = allMvcNames
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
		{
			if (ModelState.IsValid)
			{
				var role = new IdentityRole(model.RoleName);
				var result = await _roleManager.CreateAsync(role);
				if (result.Succeeded)
				{
					var requestRoles = model.ActionAndControllerNames.Where(c => c.IsSelected).ToList();
					foreach (var requestRole in requestRoles)
					{
						var areaName = string.IsNullOrEmpty(requestRole.AreaName) ? "NoArea" : requestRole.AreaName;

						await _roleManager.AddClaimAsync
							(role, new Claim($"{areaName}|{requestRole.ControllerName}|{requestRole.ActionName}"
							, true.ToString()));
					}
					return RedirectToAction("Index");
				}
				foreach (var er in result.Errors)
				{
					ModelState.AddModelError("", er.Description);
				}
			}

			return View(model);
		}
	}
}
