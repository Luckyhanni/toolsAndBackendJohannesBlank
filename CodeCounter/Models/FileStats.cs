namespace CodeCounter.Models;

public class FileStats
{
    public string FilePath { get; set; } = string.Empty;

    public int CodeLines { get; set; }

    public int ClassCount { get; set; }

    public int PartialClassCount { get; set; }

    public List<string> ClassNames { get; set; } = new();

    public List<string> PartialClassNames { get; set; } = new();
}
