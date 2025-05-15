namespace MyKpiyapProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbAdminLogs",
                c => new
                    {
                        LogID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(nullable: false),
                        Action = c.String(),
                        EventType = c.String(),
                        DateTime = c.DateTime(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.LogID)
                .ForeignKey("dbo.tbEmployees", t => t.EmployeeID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.tbEmployees",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Gender = c.String(),
                        Login = c.String(),
                        Password = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Photo = c.Binary(),
                        PositionAndRole = c.String(),
                        Experience = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeID);
            
            CreateTable(
                "dbo.tbProjects",
                c => new
                    {
                        ProjectID = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        ClosingDate = c.DateTime(nullable: false),
                        Status = c.String(),
                        Description = c.String(),
                        Title = c.String(),
                        DocxData = c.Binary(),
                        CreatorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectID)
                .ForeignKey("dbo.tbEmployees", t => t.CreatorID)
                .Index(t => t.CreatorID);
            
            CreateTable(
                "dbo.tbTasks",
                c => new
                    {
                        TaskID = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        Status = c.String(),
                        Description = c.String(),
                        Title = c.String(),
                        ProjectID = c.Int(nullable: false),
                        Priority = c.String(),
                        ExecutorID = c.Int(nullable: false),
                        CreatorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaskID)
                .ForeignKey("dbo.tbEmployees", t => t.CreatorID, cascadeDelete: true)
                .ForeignKey("dbo.tbProjects", t => t.ProjectID, cascadeDelete: true)
                .ForeignKey("dbo.tbEmployees", t => t.ExecutorID)
                .Index(t => t.ProjectID)
                .Index(t => t.ExecutorID)
                .Index(t => t.CreatorID);
            
            CreateTable(
                "dbo.tbReports",
                c => new
                    {
                        ReportID = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        EmployeeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReportID)
                .ForeignKey("dbo.tbEmployees", t => t.EmployeeID)
                .Index(t => t.EmployeeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbTasks", "ExecutorID", "dbo.tbEmployees");
            DropForeignKey("dbo.tbReports", "EmployeeID", "dbo.tbEmployees");
            DropForeignKey("dbo.tbProjects", "CreatorID", "dbo.tbEmployees");
            DropForeignKey("dbo.tbTasks", "ProjectID", "dbo.tbProjects");
            DropForeignKey("dbo.tbTasks", "CreatorID", "dbo.tbEmployees");
            DropForeignKey("dbo.tbAdminLogs", "EmployeeID", "dbo.tbEmployees");
            DropIndex("dbo.tbReports", new[] { "EmployeeID" });
            DropIndex("dbo.tbTasks", new[] { "CreatorID" });
            DropIndex("dbo.tbTasks", new[] { "ExecutorID" });
            DropIndex("dbo.tbTasks", new[] { "ProjectID" });
            DropIndex("dbo.tbProjects", new[] { "CreatorID" });
            DropIndex("dbo.tbAdminLogs", new[] { "EmployeeID" });
            DropTable("dbo.tbReports");
            DropTable("dbo.tbTasks");
            DropTable("dbo.tbProjects");
            DropTable("dbo.tbEmployees");
            DropTable("dbo.tbAdminLogs");
        }
    }
}
