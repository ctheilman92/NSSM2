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
            ContextKey = "NSSM2.Core.NSContext";
        }

        protected override void Seed(NSSM2.Core.NSContext context)
        {

        }
    }
}
