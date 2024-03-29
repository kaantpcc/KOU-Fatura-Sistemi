namespace BillManagementSystem.Models.Entities
{
    public class Bill
    {
        public Guid Id { get; set; }
        public DateTime BillDateTime { get; set; }
        public string BillDescription { get; set; }
        public string BillName { get; set;}
        public string BillDepartment { get; set;}
        public int BillValue { get; set;}
        public string BillImage { get; set;}
    }
}
