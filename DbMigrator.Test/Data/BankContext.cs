using System.Data.Entity;

namespace DbMigrator.Test.Data
{
    public class BankContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public BankContext()
            : base("Data Source=.;Initial Catalog=DbNugetTest3;Integrated Security=True")
        {
            Database.SetInitializer<BankContext>(null);
        }
    }
}
