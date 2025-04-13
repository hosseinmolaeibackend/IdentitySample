using IdentitySample.Context;
using IdentitySample.Models;
using IdentitySample.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IdentitySample.Controllers
{
	public class SiteSettingController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IMemoryCache _memoryCache;
		public SiteSettingController(ApplicationDbContext context, IMemoryCache memoryCache)
		{
			_context = context;
			_memoryCache = memoryCache;
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
				_context.SiteSettings.FirstOrDefault(t => t.Key == "RoleValidateGuid");
			var model = new RoleValidateGuidViewModel()
			{
				Value = roleValidationGuidSiteSetting.Value,
				LastChangeTime = roleValidationGuidSiteSetting?.LastTimeChanged
			};
			return View(model);
		}

		[HttpPost]
		public IActionResult RoleValidationGuid(RoleValidateGuidViewModel model)
		{
			var roleValidationGuidSiteSetting =
				_context.SiteSettings.FirstOrDefault(t => t.Key == "RoleValidationGuid");
			if (roleValidationGuidSiteSetting == null)
			{
				_context.SiteSettings.Add(new SiteSetting()
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
			_memoryCache.Remove("RoleValidationGuid");

			return RedirectToAction("Index");
		}
	}
}
