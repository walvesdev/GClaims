using FluentMigrator;
using GClaims.Marvel.Migrator.Extensions;

namespace GClaims.Marvel.Migrator.Migrations;

[Migration(20220721101401)]
public class Initial_Marvel: AppMigration
{
    public override void Up()
    {
        CreateTable("Account", IdentityType.Guid)
            .WithColumn("Name").AsString()
            .WithColumn("Password").AsString()
            .WithColumn("RoleId").AsInt32();
    }
    
    public override void Down()
    {
        Delete.Table("Marvel");
    }
}