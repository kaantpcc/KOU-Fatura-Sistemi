using System.ComponentModel.DataAnnotations;

namespace BillManagementSystem.Models.Entities
{
	public class Account
	{
		[Key]
		public string Email { get; set; }
		public string Password {  get; set; }
		public string FName { get; set; }
		public string LName { get; set; }
	}
}
