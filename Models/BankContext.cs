using Microsoft.EntityFrameworkCore;

namespace Bank_Branches_Project.Models
{
    public class BankContext : DbContext
    {
        public DbSet<BankBranch> BankBranches { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public BankContext(DbContextOptions<BankContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=asp.db");
                base.OnConfiguring(optionsBuilder);
            }         
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Employee>().HasIndex(r => r.CivilId).IsUnique();
        }
    }
}
