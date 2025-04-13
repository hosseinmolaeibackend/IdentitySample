using IdentitySample.Context;
using IdentitySample.Models;
using IdentitySample.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IdentitySample.Utitlities
{
	public class Utitlities : IUtilities
	{
		private readonly ApplicationDbContext _context;
		public Utitlities(ApplicationDbContext context)
		{
			_context = context;
		}

		public IList<ActionAndControllerName> ActionAndControllerNameList()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			var contradistinction = asm.GetTypes()
				.Where(type => typeof(Controller).IsAssignableFrom(type))
				.SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
				.Select(x => new
				{
					Controller = x.DeclaringType?.Name,
					Action = x.Name,
					Area = x.DeclaringType?.CustomAttributes.Where(c => c.AttributeType == typeof(AreaAttribute))
				});
			var list = new List<ActionAndControllerName>();
			foreach (var action in contradistinction)
			{
				if (action.Area.Count() != 0)
				{
					list.Add(new ActionAndControllerName()
					{
						ControllerName = action.Controller,
						ActionName = action.Action,
						AreaName = action.Area.Select(v => v.ConstructorArguments[0].Value.ToString()).FirstOrDefault()
					});
				}
				else
				{
					list.Add(new ActionAndControllerName()
					{
						ControllerName = action.Controller,
						ActionName = action.Action,
						AreaName = null
					});
				}
			}

			return list.Distinct().ToList();
		}

		public IList<string> GetAllAreaNames()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			var contradistinction = asm.GetTypes()
				.Where(type => typeof(Controller).IsAssignableFrom(type))
				.SelectMany(type =>
					type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
				.Select(x => new
				{
					Area = x.DeclaringType?.CustomAttributes.Where(c => c.AttributeType == typeof(AreaAttribute))

				});

			var list = new List<string>();

			foreach (var item in contradistinction)
			{
				list.Add(item.Area.Select(v => v.ConstructorArguments[0].Value.ToString()).FirstOrDefault());
			}

			if (list.All(string.IsNullOrEmpty))
			{
				return new List<string>();
			}

			list.RemoveAll(x => x == null);

			return list.Distinct().ToList();
		}

		public string DataBaseRoleValidationGuid()
		{
			var roleValidationGuid =
			   _context.SiteSettings.SingleOrDefault(s => s.Key == "RoleValidationGuid")?.Value;

			while (roleValidationGuid == null)
			{
				_context.SiteSettings.Add(new SiteSetting()
				{
					Key = "RoleValidationGuid",
					Value = Guid.NewGuid().ToString(),
					LastTimeChanged = DateTime.Now
				});
				_context.SaveChanges();
				roleValidationGuid =
					_context.SiteSettings.SingleOrDefault(s => s.Key == "RoleValidationGuid")?.Value;
			}

			return roleValidationGuid;
		}
	}
}
