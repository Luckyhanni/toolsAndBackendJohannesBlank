using System.Text.Json;
using CodeCounter.Models;

namespace CodeCounter.Services;

public class ReportBuilder
{
    public ProjectStats BuildProjectStats(string rootPath, IEnumerable<FileStats> fileStats)
    {
        var projectStats = new ProjectStats
        {
            RootPath = rootPath,
            Files = fileStats.ToList()
        };

        projectStats.TotalFiles = projectStats.Files.Count;
        projectStats.TotalCodeLines = projectStats.Files.Sum(f => f.CodeLines);
        projectStats.TotalPhysicalClasses = projectStats.Files.Sum(f => f.ClassCount);
        projectStats.TotalPartialClassDeclarations = projectStats.Files.Sum(f => f.PartialClassCount);

        var classGroups = new Dictionary<string, ClassGroupStats>(StringComparer.Ordinal);

        foreach (var file in projectStats.Files)
        {
            foreach (var name in file.ClassNames.Distinct())
            {
                if (!classGroups.TryGetValue(name, out var group))
                {
                    group = new ClassGroupStats { ClassName = name };
                    classGroups[name] = group;
                }

                group.Declarations++;
            }

            foreach (var partialName in file.PartialClassNames.Distinct())
            {
                if (!classGroups.TryGetValue(partialName, out var group))
                {
                    group = new ClassGroupStats { ClassName = partialName };
                    classGroups[partialName] = group;
                }

                group.IsPartial = true;
            }
        }

        projectStats.ClassGroups = classGroups.Values
            .OrderBy(g => g.ClassName, StringComparer.Ordinal)
            .ToList();

        projectStats.TotalLogicalClasses = projectStats.ClassGroups.Count;
        projectStats.PartialClassGroups = projectStats.ClassGroups.Count(g => g.IsPartial);

        return projectStats;
    }

    public void SaveJson(ProjectStats stats, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(stats, options);
        File.WriteAllText(filePath, json);
    }
}
