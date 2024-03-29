using BillManagementSystem.Data;
using BillManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace BillManagementSystem.Controllers
{
	[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext dbContext;
		public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
			this.dbContext = dbContext;
			_logger = logger;
        }

		public async Task<IActionResult> Index()
		{
			// Oturum açmýþ kullanýcýnýn e-posta adresini al
			var email = User.FindFirstValue(ClaimTypes.Email);

			// E-posta adresine göre kullanýcý bilgilerini sorgula
			var account = await dbContext.Accounts
				.Where(a => a.Email == email)
				.FirstOrDefaultAsync();

			if (account != null)
			{
				// Kullanýcý bulunduysa, FName ve LName bilgilerini ViewBag üzerinden görünüme taþý
				ViewBag.FName = account.FName;
				ViewBag.LName = account.LName;	
			}

			return View();
		}


		public IActionResult Privacy()
        {
            return View();
        }

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login","Login");
		}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
