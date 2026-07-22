using FilamentDbApp.Models;
using Microsoft.Data.Sqlite;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    public List<ManufacturerRecord> LoadManufacturers()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT ManufacturerId, Name, COALESCE(DisplayName,''), COALESCE(Country,''), COALESCE(Founded,''), COALESCE(Website,''), COALESCE(LogoUrl,''), COALESCE(Description,''), COALESCE(EngineeringFocus,''), COALESCE(MaterialCategories,''), COALESCE(Strengths,''), COALESCE(Weaknesses,''), COALESCE(Sustainability,''), COALESCE(TypicalApplications,''), COALESCE(Headquarters,''), COALESCE(Notes,''), COALESCE(SortOrder,100), COALESCE(IsActive,1), COALESCE(CreatedAtUtc,''), COALESCE(UpdatedAtUtc,'') FROM Manufacturers ORDER BY SortOrder, Name;";
        using var reader = command.ExecuteReader();
        var rows = new List<ManufacturerRecord>();
        while (reader.Read())
        {
            rows.Add(new ManufacturerRecord
            {
                ManufacturerId = reader.GetInt64(0), Name = reader.GetString(1), DisplayName = reader.GetString(2), Country = reader.GetString(3), Founded = reader.GetString(4), Website = reader.GetString(5), LogoUrl = reader.GetString(6), Description = reader.GetString(7), EngineeringFocus = reader.GetString(8), MaterialCategories = reader.GetString(9), Strengths = reader.GetString(10), Weaknesses = reader.GetString(11), Sustainability = reader.GetString(12), TypicalApplications = reader.GetString(13), Headquarters = reader.GetString(14), Notes = reader.GetString(15), SortOrder = reader.GetInt32(16), IsActive = reader.GetInt32(17) != 0, CreatedAtUtc = reader.GetString(18), UpdatedAtUtc = reader.GetString(19)
            });
        }
        return rows;
    }

    public void SaveManufacturer(ManufacturerRecord row)
    {
        if (string.IsNullOrWhiteSpace(row.Name)) throw new InvalidOperationException("Manufacturer Name is required.");
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        var now = DateTime.UtcNow.ToString("O");
        command.CommandText = row.ManufacturerId == 0
            ? @"INSERT INTO Manufacturers (Name,DisplayName,Country,Founded,Website,LogoUrl,Description,EngineeringFocus,MaterialCategories,Strengths,Weaknesses,Sustainability,TypicalApplications,Headquarters,Notes,SortOrder,IsActive,CreatedAtUtc,UpdatedAtUtc) VALUES ($Name,$DisplayName,$Country,$Founded,$Website,$LogoUrl,$Description,$EngineeringFocus,$MaterialCategories,$Strengths,$Weaknesses,$Sustainability,$TypicalApplications,$Headquarters,$Notes,$SortOrder,$IsActive,$CreatedAtUtc,$UpdatedAtUtc); SELECT last_insert_rowid();"
            : @"UPDATE Manufacturers SET Name=$Name,DisplayName=$DisplayName,Country=$Country,Founded=$Founded,Website=$Website,LogoUrl=$LogoUrl,Description=$Description,EngineeringFocus=$EngineeringFocus,MaterialCategories=$MaterialCategories,Strengths=$Strengths,Weaknesses=$Weaknesses,Sustainability=$Sustainability,TypicalApplications=$TypicalApplications,Headquarters=$Headquarters,Notes=$Notes,SortOrder=$SortOrder,IsActive=$IsActive,UpdatedAtUtc=$UpdatedAtUtc WHERE ManufacturerId=$ManufacturerId; SELECT $ManufacturerId;";
        command.Parameters.AddWithValue("$ManufacturerId", row.ManufacturerId);
        command.Parameters.AddWithValue("$Name", row.Name.Trim()); command.Parameters.AddWithValue("$DisplayName", row.DisplayName ?? ""); command.Parameters.AddWithValue("$Country", row.Country ?? ""); command.Parameters.AddWithValue("$Founded", row.Founded ?? ""); command.Parameters.AddWithValue("$Website", row.Website ?? ""); command.Parameters.AddWithValue("$LogoUrl", row.LogoUrl ?? ""); command.Parameters.AddWithValue("$Description", row.Description ?? ""); command.Parameters.AddWithValue("$EngineeringFocus", row.EngineeringFocus ?? ""); command.Parameters.AddWithValue("$MaterialCategories", row.MaterialCategories ?? ""); command.Parameters.AddWithValue("$Strengths", row.Strengths ?? ""); command.Parameters.AddWithValue("$Weaknesses", row.Weaknesses ?? ""); command.Parameters.AddWithValue("$Sustainability", row.Sustainability ?? ""); command.Parameters.AddWithValue("$TypicalApplications", row.TypicalApplications ?? ""); command.Parameters.AddWithValue("$Headquarters", row.Headquarters ?? ""); command.Parameters.AddWithValue("$Notes", row.Notes ?? ""); command.Parameters.AddWithValue("$SortOrder", row.SortOrder); command.Parameters.AddWithValue("$IsActive", row.IsActive ? 1 : 0); command.Parameters.AddWithValue("$CreatedAtUtc", string.IsNullOrWhiteSpace(row.CreatedAtUtc) ? now : row.CreatedAtUtc); command.Parameters.AddWithValue("$UpdatedAtUtc", now);
        row.ManufacturerId = Convert.ToInt64(command.ExecuteScalar()); row.UpdatedAtUtc = now; if (string.IsNullOrWhiteSpace(row.CreatedAtUtc)) row.CreatedAtUtc = now;
    }

    public void DeleteManufacturer(long manufacturerId)
    {
        CreateAutomaticBackupBeforeMajorChange();
        using var connection = new SqliteConnection(ConnectionString); connection.Open();
        using var command = connection.CreateCommand(); command.CommandText = "DELETE FROM Manufacturers WHERE ManufacturerId=$id;"; command.Parameters.AddWithValue("$id", manufacturerId); command.ExecuteNonQuery();
    }
}
