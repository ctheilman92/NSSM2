namespace NSSM.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NS_MEMBERS",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FIRST_NAME = c.String(nullable: false),
                        LAST_NAME = c.String(nullable: false),
                        EMAIL = c.String(nullable: false),
                        PHONE = c.String(),
                        IS_ACTIVE = c.Boolean(nullable: false),
                        IS_DELETED = c.Boolean(nullable: false),
                        CREATED_DATE = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        MODIFIED_DATE = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.NS_NODES",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ALIAS = c.String(nullable: false),
                        FQDN = c.String(),
                        DOMAIN = c.String(),
                        NS_EXE_LOCATION = c.String(nullable: false),
                        CONCURRENT_SCANS = c.Int(nullable: false),
                        START_TIME = c.DateTime(precision: 7, storeType: "datetime2"),
                        STOP_TIME = c.DateTime(precision: 7, storeType: "datetime2"),
                        ADMIN_ID = c.Int(nullable: false),
                        CREATED_BY = c.Int(nullable: false),
                        IS_ACTIVE = c.Boolean(nullable: false),
                        IS_DELETED = c.Boolean(nullable: false),
                        CREATED_DATE = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        MODIFIED_DATE = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_MEMBERS", t => t.ADMIN_ID)
                .ForeignKey("dbo.NS_MEMBERS", t => t.CREATED_BY)
                .Index(t => t.ADMIN_ID)
                .Index(t => t.CREATED_BY);
            
            CreateTable(
                "dbo.NS_SCANS",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SCAN_ALIAS = c.String(),
                        STATUS = c.Int(nullable: false),
                        TARGET_URL = c.String(),
                        EXPORT_PATH = c.String(),
                        INVOKE_DATE = c.DateTime(precision: 7, storeType: "datetime2"),
                        END_DATE = c.DateTime(precision: 7, storeType: "datetime2"),
                        TIMEOUT = c.Int(),
                        RETRY_ON_FAIL = c.Boolean(nullable: false),
                        ERROR = c.String(),
                        NODE_ID = c.Int(),
                        PROJECT_ID = c.Int(nullable: false),
                        IS_ACTIVE = c.Boolean(nullable: false),
                        IS_DELETED = c.Boolean(nullable: false),
                        CREATED_DATE = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        MODIFIED_DATE = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NODES", t => t.NODE_ID)
                .ForeignKey("dbo.NS_PROJECTS", t => t.PROJECT_ID, cascadeDelete: true)
                .Index(t => t.NODE_ID)
                .Index(t => t.PROJECT_ID);
            
            CreateTable(
                "dbo.NS_PROJECTS",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PROJECT_NAME = c.String(),
                        SUMMARY_LOCATION = c.String(),
                        CREATED_BY = c.Int(nullable: false),
                        IS_ACTIVE = c.Boolean(nullable: false),
                        IS_DELETED = c.Boolean(nullable: false),
                        CREATED_DATE = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        MODIFIED_DATE = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_MEMBERS", t => t.CREATED_BY, cascadeDelete: true)
                .Index(t => t.CREATED_BY);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NS_SCANS", "PROJECT_ID", "dbo.NS_PROJECTS");
            DropForeignKey("dbo.NS_PROJECTS", "CREATED_BY", "dbo.NS_MEMBERS");
            DropForeignKey("dbo.NS_SCANS", "NODE_ID", "dbo.NS_NODES");
            DropForeignKey("dbo.NS_NODES", "CREATED_BY", "dbo.NS_MEMBERS");
            DropForeignKey("dbo.NS_NODES", "ADMIN_ID", "dbo.NS_MEMBERS");
            DropIndex("dbo.NS_PROJECTS", new[] { "CREATED_BY" });
            DropIndex("dbo.NS_SCANS", new[] { "PROJECT_ID" });
            DropIndex("dbo.NS_SCANS", new[] { "NODE_ID" });
            DropIndex("dbo.NS_NODES", new[] { "CREATED_BY" });
            DropIndex("dbo.NS_NODES", new[] { "ADMIN_ID" });
            DropTable("dbo.NS_PROJECTS");
            DropTable("dbo.NS_SCANS");
            DropTable("dbo.NS_NODES");
            DropTable("dbo.NS_MEMBERS");
        }
    }
}
