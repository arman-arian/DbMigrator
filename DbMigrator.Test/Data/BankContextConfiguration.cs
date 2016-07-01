using System.Data.Entity.Migrations;

namespace DbMigrator.Test.Data
{
    internal sealed class BankContextContextConfiguration : DbMigrationsConfiguration<BankContext>
    {
        public BankContextContextConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            CommandTimeout = 180;
        }

        protected override void Seed(BankContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
