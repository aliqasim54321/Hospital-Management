namespace HospitalManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderpatient : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Order_id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        Category = c.String(),
                        Total_Price = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Order_id)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "PatientId", "dbo.Patients");
            DropIndex("dbo.Orders", new[] { "PatientId" });
            DropTable("dbo.Orders");
        }
    }
}
