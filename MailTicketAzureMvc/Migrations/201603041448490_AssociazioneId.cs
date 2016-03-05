namespace MailTicketAzureMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssociazioneId : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Associaziones");
            AddColumn("dbo.Associaziones", "Id_Associazione", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Associaziones", "IdUtente", c => c.String());
            AddPrimaryKey("dbo.Associaziones", "Id_Associazione");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Associaziones");
            AlterColumn("dbo.Associaziones", "IdUtente", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Associaziones", "Id_Associazione");
            AddPrimaryKey("dbo.Associaziones", "IdUtente");
        }
    }
}
