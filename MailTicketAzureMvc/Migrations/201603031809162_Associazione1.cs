namespace MailTicketAzureMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Associazione1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Associaziones", "IdSpettacolo", c => c.String());
            AddColumn("dbo.Associaziones", "IdEvento", c => c.String());
            AddColumn("dbo.Associaziones", "IdSpettMail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Associaziones", "IdSpettMail");
            DropColumn("dbo.Associaziones", "IdEvento");
            DropColumn("dbo.Associaziones", "IdSpettacolo");
        }
    }
}
