namespace HospitalManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Income : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Incomes",
                c => new
                    {
                        income_id = c.Int(nullable: false, identity: true),
                        pay_type = c.String(),
                        Order_id = c.Int(nullable: false),
                        income_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.income_id)
                .ForeignKey("dbo.Orders", t => t.Order_id, cascadeDelete: true)
                .Index(t => t.Order_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Incomes", "Order_id", "dbo.Orders");
            DropIndex("dbo.Incomes", new[] { "Order_id" });
            DropTable("dbo.Incomes");
        }
    }
}
