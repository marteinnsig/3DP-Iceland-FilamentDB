using ClosedXML.Excel;
using FilamentDbApp.Models;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace FilamentDbApp.Services;

public sealed class ExcelDisasterRecoveryService
{
    public const string ManifestSheetName = "DR Manifest";
    public const string PackageIdentity = "3DPIceland Excel Disaster Recovery";
    private const string NullMarker = "~NULL";
    private const string TextPrefix = "~UTF8:";
    private const string BlobPrefix = "~BASE64:";
    private const int ExcelChunkLength = 30000;

    public void AddRecoveryPackage(XLWorkbook workbook, ExcelRecoverySnapshot snapshot)
    {
        var manifest = workbook.Worksheets.Add(ManifestSheetName);
        manifest.Cell(1, 1).Value = PackageIdentity;
        manifest.Cell(2, 1).Value = "Format version"; manifest.Cell(2, 2).Value = snapshot.FormatVersion;
        manifest.Cell(3, 1).Value = "Source schema"; manifest.Cell(3, 2).Value = snapshot.SourceSchemaVersion.ToString(CultureInfo.InvariantCulture);
        manifest.Cell(4, 1).Value = "Exported UTC"; manifest.Cell(4, 2).Value = snapshot.ExportedAtUtc;
        manifest.Cell(6, 1).Value = "Table"; manifest.Cell(6, 2).Value = "Sheet"; manifest.Cell(6, 3).Value = "Rows"; manifest.Cell(6, 4).Value = "Columns"; manifest.Cell(6, 5).Value = "SHA-256";
        manifest.Range(1, 1, 1, 5).Style.Font.Bold = true;
        manifest.Range(6, 1, 6, 5).Style.Font.Bold = true;

        var manifestRow = 7;
        foreach (var table in snapshot.Tables)
        {
            table.Sha256 = ComputeTableHash(table);
            var sheet = workbook.Worksheets.Add(table.SheetName);
            sheet.Cell(1, 1).Value = PackageIdentity; sheet.Cell(1, 2).Value = table.TableName;
            sheet.Cell(2, 1).Value = "Columns"; sheet.Cell(2, 2).Value = string.Join("\u001f", table.Columns);
            sheet.Cell(4, 1).Value = "Data row"; sheet.Cell(4, 2).Value = "Column"; sheet.Cell(4, 3).Value = "Chunk"; sheet.Cell(4, 4).Value = "Chunks"; sheet.Cell(4, 5).Value = "Encoded data";
            sheet.Range(1, 1, 1, 5).Style.Font.Bold = true;
            sheet.Range(4, 1, 4, 5).Style.Font.Bold = true;
            var physicalRow = 5;
            for (var row = 0; row < table.Rows.Count; row++)
            {
                for (var column = 0; column < table.Columns.Count; column++)
                {
                    var encoded = EncodeCell(table.Rows[row][column]);
                    var chunkCount = Math.Max(1, (int)Math.Ceiling(encoded.Length / (double)ExcelChunkLength));
                    for (var chunk = 0; chunk < chunkCount; chunk++)
                    {
                        var start = chunk * ExcelChunkLength;
                        var length = Math.Min(ExcelChunkLength, encoded.Length - start);
                        sheet.Cell(physicalRow, 1).Value = row.ToString(CultureInfo.InvariantCulture);
                        sheet.Cell(physicalRow, 2).Value = column.ToString(CultureInfo.InvariantCulture);
                        sheet.Cell(physicalRow, 3).Value = chunk.ToString(CultureInfo.InvariantCulture);
                        sheet.Cell(physicalRow, 4).Value = chunkCount.ToString(CultureInfo.InvariantCulture);
                        sheet.Cell(physicalRow, 5).Value = encoded.Substring(start, length);
                        physicalRow++;
                    }
                }
            }
            sheet.SheetView.FreezeRows(4);

            manifest.Cell(manifestRow, 1).Value = table.TableName;
            manifest.Cell(manifestRow, 2).Value = table.SheetName;
            manifest.Cell(manifestRow, 3).Value = table.Rows.Count.ToString(CultureInfo.InvariantCulture);
            manifest.Cell(manifestRow, 4).Value = table.Columns.Count.ToString(CultureInfo.InvariantCulture);
            manifest.Cell(manifestRow, 5).Value = table.Sha256;
            manifestRow++;
        }
        manifest.Columns().AdjustToContents();
    }

    public ExcelRecoverySnapshot LoadAndVerify(string workbookPath)
    {
        using var workbook = new XLWorkbook(workbookPath);
        var manifest = workbook.Worksheets.FirstOrDefault(sheet => string.Equals(sheet.Name, ManifestSheetName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("This workbook has no governed disaster-recovery manifest.");
        if (!string.Equals(manifest.Cell(1, 1).GetString().Trim(), PackageIdentity, StringComparison.Ordinal))
            throw new InvalidOperationException("The Excel disaster-recovery package identity is invalid.");
        var format = manifest.Cell(2, 2).GetString().Trim();
        if (!string.Equals(format, ExcelRecoverySnapshot.CurrentFormatVersion, StringComparison.Ordinal))
            throw new InvalidOperationException("Unsupported Excel disaster-recovery format version: " + format);
        if (!int.TryParse(manifest.Cell(3, 2).GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var schema) || schema <= 0)
            throw new InvalidOperationException("The recovery manifest has no valid source schema version.");

        var snapshot = new ExcelRecoverySnapshot { FormatVersion = format, SourceSchemaVersion = schema, ExportedAtUtc = manifest.Cell(4, 2).GetString().Trim() };
        var row = 7;
        while (!string.IsNullOrWhiteSpace(manifest.Cell(row, 1).GetString()))
        {
            var tableName = manifest.Cell(row, 1).GetString().Trim();
            var sheetName = manifest.Cell(row, 2).GetString().Trim();
            if (!int.TryParse(manifest.Cell(row, 3).GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var expectedRows) || expectedRows < 0 ||
                !int.TryParse(manifest.Cell(row, 4).GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var expectedColumns) || expectedColumns <= 0)
                throw new InvalidOperationException("Invalid row/column counts in the recovery manifest for " + tableName + ".");
            var expectedHash = manifest.Cell(row, 5).GetString().Trim();
            var sheet = workbook.Worksheets.FirstOrDefault(item => string.Equals(item.Name, sheetName, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Missing recovery sheet: " + sheetName);
            var table = new ExcelRecoveryTable { TableName = tableName, SheetName = sheetName };
            if (!string.Equals(sheet.Cell(1, 1).GetString(), PackageIdentity, StringComparison.Ordinal) || !string.Equals(sheet.Cell(1, 2).GetString(), tableName, StringComparison.Ordinal))
                throw new InvalidOperationException("Recovery sheet identity is invalid: " + sheetName);
            table.Columns.AddRange(sheet.Cell(2, 2).GetString().Split('\u001f'));
            if (table.Columns.Any(string.IsNullOrWhiteSpace) || table.Columns.Distinct(StringComparer.OrdinalIgnoreCase).Count() != table.Columns.Count)
                throw new InvalidOperationException("Invalid or duplicate column names in " + sheetName + ".");
            if (table.Columns.Count != expectedColumns) throw new InvalidOperationException("Recovery column count mismatch in " + sheetName + ".");
            var chunks = new Dictionary<(int Row, int Column), (int Count, SortedDictionary<int, string> Values)>();
            var physicalRow = 5;
            while (!string.IsNullOrWhiteSpace(sheet.Cell(physicalRow, 1).GetString()))
            {
                if (!int.TryParse(sheet.Cell(physicalRow, 1).GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var dataRow) || dataRow < 0 || dataRow >= expectedRows ||
                    !int.TryParse(sheet.Cell(physicalRow, 2).GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var column) || column < 0 || column >= expectedColumns ||
                    !int.TryParse(sheet.Cell(physicalRow, 3).GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var chunk) || chunk < 0 ||
                    !int.TryParse(sheet.Cell(physicalRow, 4).GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var chunkCount) || chunkCount <= 0 || chunk >= chunkCount)
                    throw new InvalidOperationException("Invalid chunk coordinates in " + sheetName + ".");
                var key = (dataRow, column);
                if (!chunks.TryGetValue(key, out var entry)) entry = (chunkCount, new SortedDictionary<int, string>());
                if (entry.Count != chunkCount || !entry.Values.TryAdd(chunk, sheet.Cell(physicalRow, 5).GetString()))
                    throw new InvalidOperationException("Duplicate or inconsistent chunks in " + sheetName + ".");
                chunks[key] = entry;
                physicalRow++;
            }
            for (var dataRow = 0; dataRow < expectedRows; dataRow++)
            {
                var values = new List<object?>();
                for (var column = 0; column < expectedColumns; column++)
                {
                    if (!chunks.TryGetValue((dataRow, column), out var entry) || entry.Values.Count != entry.Count || entry.Values.Keys.Where((value, index) => value != index).Any())
                        throw new InvalidOperationException("Missing recovery chunks in " + sheetName + ".");
                    values.Add(DecodeCell(string.Concat(entry.Values.Values)));
                }
                table.Rows.Add(values);
            }
            table.Sha256 = ComputeTableHash(table);
            if (!string.Equals(table.Sha256, expectedHash, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("SHA-256 verification failed for recovery table " + tableName + ".");
            snapshot.Tables.Add(table);
            row++;
        }
        if (snapshot.Tables.Count == 0) throw new InvalidOperationException("The recovery workbook contains no governed tables.");
        return snapshot;
    }

    public static string ComputeTableHash(ExcelRecoveryTable table)
    {
        var builder = new StringBuilder();
        builder.Append(table.TableName).Append('\n');
        foreach (var column in table.Columns) builder.Append(EncodeHashToken(column)).Append('\n');
        foreach (var row in table.Rows)
            foreach (var value in row) builder.Append(EncodeHashToken(EncodeCell(value))).Append('\n');
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString()))).ToLowerInvariant();
    }

    private static string EncodeCell(object? value) => value switch
    {
        null or DBNull => NullMarker,
        byte[] bytes => BlobPrefix + Convert.ToBase64String(bytes),
        _ => TextPrefix + Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty))
    };

    private static object? DecodeCell(string value)
    {
        if (string.Equals(value, NullMarker, StringComparison.Ordinal)) return null;
        if (value.StartsWith(BlobPrefix, StringComparison.Ordinal)) return Convert.FromBase64String(value[BlobPrefix.Length..]);
        if (value.StartsWith(TextPrefix, StringComparison.Ordinal)) return Encoding.UTF8.GetString(Convert.FromBase64String(value[TextPrefix.Length..]));
        throw new InvalidOperationException("A recovery cell has invalid type encoding.");
    }

    private static string EncodeHashToken(string value) => value.Length.ToString(CultureInfo.InvariantCulture) + ":" + value;
}
