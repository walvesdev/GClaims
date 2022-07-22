using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace GClaims.Marvel.Migrator.Extensions;

public abstract class AppMigration : Migration
{
    protected enum IdentityType
    {
        Guid = 0,
        Int = 1,
        Long = 2,
        String = 3,
        Complex = 4,
        NoIdentityAudit = 5,
        NoIdentityNoAudit = 6
    }

    protected ICreateTableColumnOptionOrWithColumnSyntax CreateTable(string tableName,
        IdentityType identityType = IdentityType.Guid, string custumIdentity = null)
    {
        var expression = Create.Table(tableName);

        switch (identityType)
        {
            case IdentityType.Guid:
                expression
                    .WithColumn("Id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewSequentialId)
                    .WithColumn("CreatorId").AsGuid().NotNullable()
                    .WithColumn("ModifierId").AsGuid().Nullable()
                    .WithColumn("DeleterId").AsGuid().Nullable();
                break;
            case IdentityType.Int:
                expression.WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("CreatorId").AsInt32().NotNullable()
                    .WithColumn("ModifierId").AsInt32().Nullable()
                    .WithColumn("DeleterId").AsInt32().Nullable();
                break;
            case IdentityType.Long:
                expression.WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("CreatorId").AsInt64().NotNullable()
                    .WithColumn("ModifierId").AsInt64().Nullable()
                    .WithColumn("DeleterId").AsInt64().Nullable();
                break;
            case IdentityType.String:
                expression.WithColumn("Id").AsString().PrimaryKey()
                    .WithColumn("CreatorId").AsString().NotNullable()
                    .WithColumn("ModifierId").AsString().Nullable()
                    .WithColumn("DeleterId").AsString().Nullable();
                break;
            case IdentityType.Complex:
                expression.WithColumn("Id").AsCustom(custumIdentity).PrimaryKey()
                    .WithColumn("CreatorId").AsCustom(custumIdentity).NotNullable()
                    .WithColumn("ModifierId").AsCustom(custumIdentity).Nullable()
                    .WithColumn("DeleterId").AsCustom(custumIdentity).Nullable();
                break;
            case IdentityType.NoIdentityAudit:
                expression
                    .WithColumn("CreatorId").AsCustom(custumIdentity).NotNullable()
                    .WithColumn("ModifierId").AsCustom(custumIdentity).Nullable()
                    .WithColumn("DeleterId").AsCustom(custumIdentity).Nullable();
                break;
            case IdentityType.NoIdentityNoAudit:
                break;
            default:
                expression.WithColumn("Id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewSequentialId)
                    .WithColumn("CreatorId").AsGuid().NotNullable()
                    .WithColumn("ModifierId").AsGuid().Nullable()
                    .WithColumn("DeleterId").AsGuid().Nullable();
                break;
        }

        return expression
            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("UpdatedAt").AsDateTime().Nullable()
            .WithColumn("DeletedAt").AsDateTime().Nullable()
            .WithColumn("IsDeleted").AsBoolean().NotNullable()
            .WithColumn("ExternalId").AsString().Nullable()
            .WithColumn("TenantId").AsGuid().Nullable();
    }
}