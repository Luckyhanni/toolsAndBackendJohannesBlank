using CodeCounter.Models;
using CodeCounter.Services;

namespace CodeCounter;

public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            if (args.Length == 0 || args.Contains("--help"))
            {
                PrintUsage();
                return 0;
            }

            string rootPath = args[0];
            bool verbose = args.Contains("--verbose");

            Console.WriteLine("CodeCounter CLI");
            Console.WriteLine("----------------");
            Console.WriteLine($"Root-Pfad: {rootPath}");
            Console.WriteLine();

            var scanner = new FileScanner();
            IEnumerable<string> files = scanner.GetCsFiles(rootPath);

            var fileList = files.ToList();
            if (fileList.Count == 0)
            {
                Console.WriteLine("Es wurden keine .cs-Dateien gefunden.");
                return 0;
            }

            var parser = new CodeParser();
            var allFileStats = new List<FileStats>();

            foreach (var file in fileList)
            {
                if (verbose)
                {
                    Console.WriteLine($"Analysiere: {file}");
                }

                var stats = parser.AnalyzeFile(file);
                allFileStats.Add(stats);
            }

            var reportBuilder = new ReportBuilder();
            ProjectStats projectStats = reportBuilder.BuildProjectStats(rootPath, allFileStats);

            Console.WriteLine();
            PrintSummary(projectStats);

            string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "code_report.json");
            reportBuilder.SaveJson(projectStats, jsonPath);

            Console.WriteLine();
            Console.WriteLine($"JSON-Report geschrieben nach: {jsonPath}");

            if (verbose)
            {
                Console.WriteLine();
                PrintTopFiles(projectStats, top: 5);
            }

            return 0;
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.Error.WriteLine($"[Fehler] {ex.Message}");
            return 1;
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"[Fehler] {ex.Message}");
            return 1;
        }
        catch (ApplicationException ex)
        {
            Console.Error.WriteLine($"[Fehler] {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("[Unerwarteter Fehler]");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("CodeCounter CLI");
        Console.WriteLine("Verwendung:");
        Console.WriteLine("  CodeCounter <Pfad> [--verbose]");
        Console.WriteLine();
        Console.WriteLine("Argumente:");
        Console.WriteLine("  <Pfad>       Wurzelverzeichnis, das rekursiv nach .cs-Dateien durchsucht wird.");
        Console.WriteLine("  --verbose    Zeigt zusätzliche Informationen zu den analysierten Dateien an.");
        Console.WriteLine("  --help       Zeigt diese Hilfe an.");
    }

    private static void PrintSummary(ProjectStats stats)
    {
        Console.WriteLine("Zusammenfassung");
        Console.WriteLine("----------------");
        Console.WriteLine($"Root-Pfad:                    {stats.RootPath}");
        Console.WriteLine($"Gefundene .cs-Dateien:        {stats.TotalFiles}");
        Console.WriteLine($"Gesamt-Codezeilen:            {stats.TotalCodeLines}");
        Console.WriteLine($"Physische Klassen (inkl.partial): {stats.TotalPhysicalClasses}");
        Console.WriteLine($"partial class Deklarationen:  {stats.TotalPartialClassDeclarations}");
        Console.WriteLine($"Logische Klassen (unique):    {stats.TotalLogicalClasses}");
        Console.WriteLine($"Davon (teilweise) partial:    {stats.PartialClassGroups}");
    }

    private static void PrintTopFiles(ProjectStats stats, int top)
    {
        Console.WriteLine($"Top {top} Dateien nach Codezeilen:");
        Console.WriteLine("-----------------------------------");

        var topFiles = stats.Files
            .OrderByDescending(f => f.CodeLines)
            .Take(top);

        foreach (var f in topFiles)
        {
            Console.WriteLine($"{f.CodeLines,6} Zeilen - {f.FilePath}");
        }
    }
}
