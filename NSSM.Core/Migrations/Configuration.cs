namespace NSSM.Core.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<NSSM2.Core.NSContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(NSSM2.Core.NSContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.Members.AddOrUpdate(x => x.Id,
                new Models.Member { Id = 1, FirstName = "Admin", LastName = "Admin", Email = "admin@admin.com" }
            );
        }
    }
}
