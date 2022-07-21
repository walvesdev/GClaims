using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace GClaims.Core.Extensions;

public static class MethodsExtension
{
    [Obsolete("Obsolete")]
    public static void ToPascalNames(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName().ToPascalCase();
            entity.SetTableName(tableName);

            foreach (var property in entity.GetProperties())
            {
                var columnName = property.Name.ToPascalCase();
                property.SetColumnName(columnName);
            }

            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName().ToPascalCase();
                key.SetName(keyName);
            }

            foreach (var key in entity.GetForeignKeys())
            {
                var foreignKeyName = key.GetConstraintName().ToPascalCase();
                key.SetConstraintName(foreignKeyName);
            }

            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.Name.ToPascalCase();
                index.SetName(indexName);
            }
        }
    }

    private static string? ToPascalCase(this string? name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? name
            : Regex.Replace(name, "(?<=[a-z])(?=[A-Z])", "");
    }
}