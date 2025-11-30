using System.Text;
using System.Text.RegularExpressions;
using CodeCounter.Models;

namespace CodeCounter.Services;

public class CodeParser
{
    private static readonly Regex ClassRegex =
        new(@"\bclass\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);

    private static readonly Regex PartialClassRegex =
        new(@"\bpartial\s+class\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);

    public FileStats AnalyzeFile(string filePath)
    {
        var stats = new FileStats { FilePath = filePath };

        string[] lines;
        try
        {
            lines = File.ReadAllLines(filePath);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Fehler beim Lesen der Datei '{filePath}': {ex.Message}", ex);
        }

        bool inBlockComment = false;

        foreach (var rawLine in lines)
        {
            var line = rawLine;
            bool codeBeforeComment;

            string lineWithoutComments = RemoveComments(line, ref inBlockComment, out codeBeforeComment);
            bool hasCode = codeBeforeComment || !string.IsNullOrWhiteSpace(lineWithoutComments);

            if (hasCode)
            {
                stats.CodeLines++;
            }

            if (!string.IsNullOrWhiteSpace(lineWithoutComments))
            {
                AnalyzeClassesInLine(lineWithoutComments, stats);
            }
        }

        return stats;
    }

    private static string RemoveComments(string line, ref bool inBlockComment, out bool codeBeforeComment)
    {
        codeBeforeComment = false;

        if (string.IsNullOrWhiteSpace(line))
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        int i = 0;

        while (i < line.Length)
        {
            if (inBlockComment)
            {
                int end = line.IndexOf("*/", i, StringComparison.Ordinal);
                if (end == -1)
                {
                    return sb.ToString();
                }

                i = end + 2;
                inBlockComment = false;
                continue;
            }

            int idxLineComment = line.IndexOf("//", i, StringComparison.Ordinal);
            int idxBlockStart = line.IndexOf("/*", i, StringComparison.Ordinal);

            int nextIndex = -1;
            string? token = null;

            if (idxLineComment >= 0 && (idxBlockStart == -1 || idxLineComment < idxBlockStart))
            {
                nextIndex = idxLineComment;
                token = "//";
            }
            else if (idxBlockStart >= 0)
            {
                nextIndex = idxBlockStart;
                token = "/*";
            }

            if (nextIndex == -1)
            {
                string rest = line.Substring(i);
                sb.Append(rest);
                if (!string.IsNullOrWhiteSpace(rest))
                {
                    codeBeforeComment = true;
                }

                break;
            }

            string chunk = line.Substring(i, nextIndex - i);
            sb.Append(chunk);
            if (!string.IsNullOrWhiteSpace(chunk))
            {
                codeBeforeComment = true;
            }

            if (token == "//")
            {
                break;
            }

            if (token == "/*")
            {
                inBlockComment = true;
                i = nextIndex + 2;
            }
        }

        return sb.ToString();
    }

    private static void AnalyzeClassesInLine(string line, FileStats stats)
    {
        foreach (Match match in PartialClassRegex.Matches(line))
        {
            string className = match.Groups[1].Value;
            stats.PartialClassCount++;
            stats.PartialClassNames.Add(className);

            stats.ClassCount++;
            stats.ClassNames.Add(className);
        }

        foreach (Match match in ClassRegex.Matches(line))
        {
            string className = match.Groups[1].Value;

            if (!stats.ClassNames.Contains(className))
            {
                stats.ClassCount++;
                stats.ClassNames.Add(className);
            }
        }
    }
}
