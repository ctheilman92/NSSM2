namespace NSSM.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nxt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NS_SCANS", "SCAN_ALIAS", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.NS_SCANS", "SCAN_ALIAS");
        }
    }
}
