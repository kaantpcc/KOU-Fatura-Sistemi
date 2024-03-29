using BillManagementSystem.Data;
using BillManagementSystem.Models.Entities;
using BillManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security.Claims;
using System.Text;

namespace BillManagementSystem.Controllers
{
	[Authorize]
	public class BillsController : Controller
	{
		private readonly ApplicationDbContext dbContext;
		private readonly IWebHostEnvironment env;
		public BillsController(ApplicationDbContext dbContext, IWebHostEnvironment env)
		{
			this.dbContext = dbContext;
			this.env = env;
		}

		[HttpGet]
		public IActionResult AddBill()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			var account = dbContext.Accounts
				.Where(a => a.Email == email)
				.FirstOrDefault();

			var departments = new List<SelectListItem>
			{
				new SelectListItem { Value = "TIP", Text = "Tıp Fakültesi" },
				new SelectListItem { Value = "MUH", Text = "Bilgisayar Mühendisliği" },
				new SelectListItem { Value = "ILT", Text = "Uluslararası İlişkiler" }
			};

			var model = new AddBillViewModel
			{
				Departments = departments,
			};

			ViewBag.FName = account.FName;
			ViewBag.LName = account.LName;

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> AddBill(AddBillViewModel viewModel)
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			var account = await dbContext.Accounts
				.Where(a => a.Email == email)
				.FirstOrDefaultAsync();

			String fileName = "";
			if (viewModel.BillImage != null)
			{
				String uploadFolder = Path.Combine(env.WebRootPath, "Images");
				fileName = viewModel.BillImage.FileName;
				String filePath = Path.Combine(uploadFolder, fileName);
				viewModel.BillImage.CopyTo(new FileStream(filePath, FileMode.Create));
			}

			var bill = new Bill
			{
				BillDateTime = DateTime.Now,
				BillDescription = viewModel.BillDescription,
				BillName = $"{account.FName} {account.LName}",
				BillDepartment = viewModel.BillDepartment,
				BillValue = viewModel.BillValue,
				BillImage = fileName,
			};

			ViewBag.FName = account.FName;
			ViewBag.LName = account.LName;


			await dbContext.Bills.AddAsync(bill);
			await dbContext.SaveChangesAsync();

			return RedirectToAction("ListBills");
		}

		[HttpGet]
		public async Task<IActionResult> ListBills()
		{
			var bills = await dbContext.Bills.ToListAsync();

			return View(bills);
		}

        [HttpGet]
        public async Task<IActionResult> FilterBills(string billDepartment)
        {
            IQueryable<Bill> billsQuery = dbContext.Bills;

            if (!string.IsNullOrEmpty(billDepartment))
            {
                billsQuery = billsQuery.Where(b => b.BillDepartment == billDepartment);
            }

            var bills = await billsQuery.ToListAsync();

            return Json(bills); // JSON olarak faturaların listesini döndür
        }

        [HttpGet]
		public async Task<IActionResult> EditBill(Guid id)
		{
			var bill = await dbContext.Bills.FindAsync(id);
			var email = User.FindFirstValue(ClaimTypes.Email);
			var account = await dbContext.Accounts
				.Where(a => a.Email == email)
				.FirstOrDefaultAsync();

			var departments = new List<SelectListItem>
			{
				new SelectListItem { Value = "TIP", Text = "Tıp Fakültesi" },
				new SelectListItem { Value = "MUH", Text = "Bilgisayar Mühendisliği" },
				new SelectListItem { Value = "ILT", Text = "Uluslararası İlişkiler" }
			};

			var model = new EditBillViewModel
			{
				Id = bill.Id,
				BillDescription = bill.BillDescription,
				BillName = bill.BillName,
				BillValue = bill.BillValue,
				BillDepartment = bill.BillDepartment,
				BillDateTime = bill.BillDateTime,
				Departments = departments,
			};

			ViewBag.FName = account.FName;
			ViewBag.LName = account.LName;

			return View(model);

		}


		[HttpPost]
		public async Task<IActionResult> EditBill(EditBillViewModel viewModel)
		{
			var bill = await dbContext.Bills.FindAsync(viewModel.Id);
			if (bill == null)
			{
				return NotFound();
			}

			if (viewModel.BillImage != null)
			{
				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.BillImage.FileName);
				var filePath = Path.Combine(env.WebRootPath, "images", fileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await viewModel.BillImage.CopyToAsync(stream);
				}

				//resmi sil
				if (!string.IsNullOrEmpty(bill.BillImage))
				{
					var oldFilePath = Path.Combine(env.WebRootPath, "images", bill.BillImage);
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
					}
				}

				bill.BillImage = fileName; // Yeni dosya adıyla güncelle
			}

			bill.BillDateTime = DateTime.Now;
			bill.BillDescription = viewModel.BillDescription;
			bill.BillName = viewModel.BillName;
			bill.BillDepartment = viewModel.BillDepartment;
			bill.BillValue = viewModel.BillValue;
		

			
			await dbContext.SaveChangesAsync();

			return RedirectToAction("ListBills");
		}

		[HttpPost]
		public async Task<IActionResult> DeleteBill(Bill viewModel)
		{
			var bill = await dbContext.Bills.FirstOrDefaultAsync(b => b.Id == viewModel.Id);

			if (bill != null)
			{
				// İlişkili resmi sil
				if (!string.IsNullOrEmpty(bill.BillImage))
				{
					var filePath = Path.Combine(env.WebRootPath, "Images", bill.BillImage);
					if (System.IO.File.Exists(filePath))
					{
						System.IO.File.Delete(filePath);
					}
				}

				// Faturayı veritabanından sil
				dbContext.Bills.Remove(bill);
				await dbContext.SaveChangesAsync();
			}

			return RedirectToAction("ListBills");
		}

        [HttpPost]
        public IActionResult ExportToExcel(string selectedDepartment)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // EPPlus lisansı
            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Faturalar");

            // Sütun başlıkları
            worksheet.Cells["A1"].Value = "Fatura ID";
            worksheet.Cells["B1"].Value = "Fatura Eklenme Tarihi";
            worksheet.Cells["C1"].Value = "Fatura Açıklaması";
            worksheet.Cells["D1"].Value = "Faturayı Ekleyen Kişi";
            worksheet.Cells["E1"].Value = "Fatura Departmanı";
            worksheet.Cells["F1"].Value = "Fatura Tutarı";

            int row = 2;

            // Filtreleme koşulu
            var billsQuery = dbContext.Bills.AsQueryable();
            if (!string.IsNullOrEmpty(selectedDepartment))
            {
                billsQuery = billsQuery.Where(b => b.BillDepartment == selectedDepartment);
            }

            foreach (var bill in billsQuery)
            {
                worksheet.Cells[row, 1].Value = bill.Id;
                worksheet.Cells[row, 2].Value = bill.BillDateTime.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 3].Value = bill.BillDescription;
                worksheet.Cells[row, 4].Value = bill.BillName;
                worksheet.Cells[row, 5].Value = bill.BillDepartment;
                worksheet.Cells[row, 6].Value = bill.BillValue.ToString("C");
                row++;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Faturalar.xlsx");
        }

    }
}
