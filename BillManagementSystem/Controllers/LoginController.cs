using BillManagementSystem.Data;
using BillManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillManagementSystem.Controllers
{
	public class LoginController : Controller
	{
		private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
			_context = context;
		}

		[HttpGet]
		public IActionResult Login()
		{
			ClaimsPrincipal claimUser = HttpContext.User;
			if(claimUser.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(Account viewModel)
		{
			var accountData = _context.Accounts.FirstOrDefault(a => a.Email == viewModel.Email && a.Password == viewModel.Password);

			if (accountData is not null)
			{
				var claims = new List<Claim>
				{
					new(ClaimTypes.Email, viewModel.Email),
				};
				
				var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				AuthenticationProperties properties = new AuthenticationProperties()
				{
					AllowRefresh = true,
					IsPersistent = false,	
				};
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(userIdentity),properties);
				return RedirectToAction("Index", "Home");
			}
			else
			{
				ViewBag.ErrorMessage = "E-posta veya şifre yanlış. Lütfen tekrar deneyiniz.";
				return View();
			}
		}
	}
}
