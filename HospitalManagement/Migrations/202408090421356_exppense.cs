namespace HospitalManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class exppense : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Expenses",
                c => new
                    {
                        expense_id = c.Int(nullable: false, identity: true),
                        exppay_type = c.String(),
                        StaffId = c.Int(nullable: false),
                        expense_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.expense_id)
                .ForeignKey("dbo.Staffs", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Expenses", "StaffId", "dbo.Staffs");
            DropIndex("dbo.Expenses", new[] { "StaffId" });
            DropTable("dbo.Expenses");
        }
    }
}
