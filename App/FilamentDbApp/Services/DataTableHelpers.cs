using System.Data;

namespace FilamentDbApp.Services;

public static class DataTableHelpers
{
    public static DataColumn? FindFirstExistingColumn(DataTable table, IEnumerable<string> possibleColumnNames)
    {
        foreach (var name in possibleColumnNames)
        {
            var column = table.Columns.Cast<DataColumn>()
                .FirstOrDefault(c => string.Equals(c.ColumnName, name, StringComparison.OrdinalIgnoreCase));
            if (column is not null) return column;
        }

        return null;
    }

    public static string FirstValue(DataRow row, params string[] possibleColumnNames)
    {
        foreach (var name in possibleColumnNames)
        {
            var column = row.Table.Columns.Cast<DataColumn>()
                .FirstOrDefault(c => string.Equals(c.ColumnName, name, StringComparison.OrdinalIgnoreCase));

            if (column is null) continue;

            var value = row[column]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(value)) return value;
        }

        return string.Empty;
    }
}
