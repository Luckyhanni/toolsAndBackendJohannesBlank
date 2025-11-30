using CodeCounter.Models;

namespace CodeCounter.Services;

public class FileScanner
{
    public IEnumerable<string> GetCsFiles(string rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
            throw new ArgumentException("Pfad darf nicht leer sein.", nameof(rootPath));

        if (!Directory.Exists(rootPath))
            throw new DirectoryNotFoundException($"Das Verzeichnis '{rootPath}' existiert nicht.");

        try
        {
            return Directory.EnumerateFiles(rootPath, "*.cs", SearchOption.AllDirectories);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new ApplicationException(
                $"Zugriff auf Unterverzeichnisse von '{rootPath}' wurde verweigert: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                $"Fehler beim Durchsuchen des Verzeichnisses '{rootPath}': {ex.Message}", ex);
        }
    }
}
