namespace CodeCounter.Models;

public class ClassGroupStats
{
    public string ClassName { get; set; } = string.Empty;

    public int Declarations { get; set; }

    public bool IsPartial { get; set; }
}
