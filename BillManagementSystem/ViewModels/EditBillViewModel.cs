using Microsoft.AspNetCore.Mvc.Rendering;

namespace BillManagementSystem.ViewModels
{
	public class EditBillViewModel
	{
		public Guid Id { get; set; }
		public DateTime BillDateTime { get; set; }
		public string BillDescription { get; set; }
		public string BillName { get; set; }
		public string BillDepartment { get; set; }
		public int BillValue { get; set; }
		public IFormFile BillImage { get; set; }
		public IEnumerable<SelectListItem> Departments { get; set; }

	}
}
