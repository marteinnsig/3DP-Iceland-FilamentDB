using System.IO;
using System.Text;

namespace FilamentDbApp.Services;

/// <summary>
/// Centralized hardened file writes for user-generated exports and diagnostics.
/// Writes to a temporary file in the destination folder and then replaces the
/// target, reducing the chance of leaving a partially-written output file.
/// </summary>
public static class SafeFileOperations
{
    public static void WriteAllTextAtomic(string path, string contents, Encoding? encoding = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Output path cannot be empty.", nameof(path));
        }

        encoding ??= new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        var fullPath = Path.GetFullPath(path);
        var folder = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrWhiteSpace(folder))
        {
            throw new IOException($"Could not determine the output folder for '{path}'.");
        }

        Directory.CreateDirectory(folder);
        var tempPath = Path.Combine(folder, $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            File.WriteAllText(tempPath, contents ?? string.Empty, encoding);
            File.Move(tempPath, fullPath, overwrite: true);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new IOException($"The application does not have permission to write this file:\n{fullPath}\n\nCheck the folder permissions or choose another folder.", ex);
        }
        catch (IOException ex)
        {
            throw new IOException(BuildFriendlyIoMessage(fullPath, ex), ex);
        }
        finally
        {
            TryDelete(tempPath);
        }
    }


    public static void WriteAllBytesAtomic(string path, byte[] contents)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Output path cannot be empty.", nameof(path));
        var fullPath = Path.GetFullPath(path);
        var folder = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrWhiteSpace(folder))
            throw new IOException($"Could not determine the output folder for '{path}'.");
        Directory.CreateDirectory(folder);
        var tempPath = Path.Combine(folder, $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");
        try
        {
            File.WriteAllBytes(tempPath, contents ?? Array.Empty<byte>());
            File.Move(tempPath, fullPath, overwrite: true);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new IOException($"The application does not have permission to write this file:\n{fullPath}\n\nCheck the folder permissions or choose another folder.", ex);
        }
        catch (IOException ex)
        {
            throw new IOException(BuildFriendlyIoMessage(fullPath, ex), ex);
        }
        finally { TryDelete(tempPath); }
    }

    public static string BuildFriendlyIoMessage(string path, IOException exception)
    {
        var fullPath = string.IsNullOrWhiteSpace(path) ? path : Path.GetFullPath(path);
        return $"The file could not be written:\n{fullPath}\n\nIt may be open or locked by another application, synchronizing through OneDrive, or the destination may be unavailable. Close programs using the file and try again.\n\nTechnical detail: {exception.Message}";
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path)) File.Delete(path);
        }
        catch
        {
            // Best-effort cleanup. A temporary file can be removed later if locked.
        }
    }
}
