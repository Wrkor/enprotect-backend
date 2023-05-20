using Microsoft.EntityFrameworkCore;
using webapi.Entities;
using webapi.Params;

namespace webapi
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Push> Pushs { get; set; }
        public DbSet<Offense> Offenses { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Role> Roles { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(@"host=localhost;port=3306;database=enprotect;username=root;password=root;");
        }
    }
}
