namespace MailTicketAzureMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMan : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.EditUserViewModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EditUserViewModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(nullable: false),
                        Nome = c.String(),
                        Telefono = c.String(),
                        CodiceFiscale = c.String(),
                        PartitaIva = c.String(),
                        Insegna = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
