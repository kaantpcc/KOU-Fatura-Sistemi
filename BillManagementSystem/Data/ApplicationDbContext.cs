using BillManagementSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            this.Database.SetCommandTimeout(999999);
        }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Account> Accounts { get; set; }
    }
}
