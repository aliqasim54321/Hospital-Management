namespace HospitalManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class patientstaff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "StaffId", c => c.Int(nullable: false));
            CreateIndex("dbo.Patients", "StaffId");
            AddForeignKey("dbo.Patients", "StaffId", "dbo.Staffs", "StaffId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients", "StaffId", "dbo.Staffs");
            DropIndex("dbo.Patients", new[] { "StaffId" });
            DropColumn("dbo.Patients", "StaffId");
        }
    }
}
