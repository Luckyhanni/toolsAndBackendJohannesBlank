namespace CodeCounter.Models;

public class ProjectStats
{
    public string RootPath { get; set; } = string.Empty;

    public int TotalFiles { get; set; }

    public int TotalCodeLines { get; set; }

    public int TotalPhysicalClasses { get; set; }

    public int TotalPartialClassDeclarations { get; set; }

    public int TotalLogicalClasses { get; set; }

    public int PartialClassGroups { get; set; }

    public List<FileStats> Files { get; set; } = new();

    public List<ClassGroupStats> ClassGroups { get; set; } = new();
}
