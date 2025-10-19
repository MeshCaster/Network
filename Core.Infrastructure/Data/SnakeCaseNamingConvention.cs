// MeshNetwork.Infrastructure/Data/Conventions/SnakeCaseNamingConvention.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Text.RegularExpressions;

namespace Core.Infrastructure.Data;

/// <summary>
/// Converts PascalCase property names to snake_case column names
/// </summary>
public class SnakeCaseNamingConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entity in modelBuilder.Metadata.GetEntityTypes())
        {
            // Convert table names to snake_case
            if (entity.GetTableName() is string tableName)
            {
                entity.SetTableName(ToSnakeCase(tableName));
            }

            // Convert column names to snake_case
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.GetColumnName()));
            }

            // Convert key names to snake_case
            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName()));
            }

            // Convert foreign key names to snake_case
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(ToSnakeCase(foreignKey.GetConstraintName()));
            }

            // Convert index names to snake_case
            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
            }
        }
    }

    private static string ToSnakeCase(string? name)
    {
        if (string.IsNullOrEmpty(name))
            return name ?? string.Empty;

        return Regex.Replace(
            Regex.Replace(
                name,
                @"([A-Z])([A-Z][a-z])",
                "$1_$2"
            ),
            @"([a-z\d])([A-Z])",
            "$1_$2"
        ).ToLower();
    }
}