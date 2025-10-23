using Microsoft.EntityFrameworkCore;

namespace PWAs.Context
{
    public class AppDbContextEmp : DbContext
    {
        private string connectionString;

        public AppDbContextEmp(DbContextOptions<AppDbContextEmp> options)
            : base(options)
        {
        }

        public AppDbContextEmp(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
