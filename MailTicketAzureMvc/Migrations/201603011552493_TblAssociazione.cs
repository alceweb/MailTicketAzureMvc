namespace MailTicketAzureMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TblAssociazione : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Associaziones",
                c => new
                    {
                        IdUtente = c.String(nullable: false, maxLength: 128),
                        IdMan = c.String(),
                    })
                .PrimaryKey(t => t.IdUtente);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Associaziones");
        }
    }
}
