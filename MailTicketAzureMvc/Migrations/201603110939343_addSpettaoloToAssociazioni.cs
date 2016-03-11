namespace MailTicketAzureMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSpettaoloToAssociazioni : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Associaziones", "Spettacolo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Associaziones", "Spettacolo");
        }
    }
}
