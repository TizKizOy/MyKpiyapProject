namespace MyKpiyapProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTaskModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbTasks", "DeadLineDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbTasks", "DeadLineDate");
        }
    }
}
