using IdentitySample.Context;
using IdentitySample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Principal;

namespace IdentitySample.Controllers
{
	public class HomeController : Controller
	{
		//private readonly IUserRepository _userRepository;
		//public HomeController(IUserRepository userRepository)
		//{
		//	_userRepository = userRepository;
		//}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}
		[Route("error/403")]
		public IActionResult AccessDenied()
		{
			return View();
		}

	}
}