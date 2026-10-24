namespace NexorisAI.Core;
public static class CleanupScanner
{
    public static string Preview()
    {
        var lines = new List<string> { "CLEANUP PREVIEW (no files are deleted)", "Old temporary files only; review paths before any future cleanup feature." };
        var roots = new[] { Path.GetTempPath(), Environment.GetEnvironmentVariable("TEMP"), Environment.GetEnvironmentVariable("TMP") }
            .Where(p => !string.IsNullOrWhiteSpace(p)).Distinct(StringComparer.OrdinalIgnoreCase);
        long total = 0; var found = 0;
        foreach (var root in roots)
        {
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root)) continue;
            foreach (var file in EnumerateFilesSafely(root))
            {
                try
                {
                    var info = new FileInfo(file);
                    if (!info.Exists || info.LastWriteTimeUtc > DateTime.UtcNow.AddHours(-24)) continue;
                    total += info.Length;
                    lines.Add("- " + info.Length.ToString("N0") + " bytes | " + info.LastWriteTime.ToString("yyyy-MM-dd HH:mm") + " | " + file);
                    if (++found >= 200) { lines.Add("Preview capped at 200 files."); goto Finished; }
                }
                catch { }
            }
        }
Finished:
        lines.Insert(2, "Candidates: " + found + "; aggregate size: " + (total / (1024.0 * 1024.0)).ToString("N2") + " MiB (estimate).");
        if (found == 0) lines.Add("No eligible files found or locations were unavailable.");
        lines.Add("No deletion, registry change, or service modification was performed.");
        return string.Join(Environment.NewLine, lines);
    }
    private static IEnumerable<string> EnumerateFilesSafely(string root)
    {
        var pending = new Stack<string>(); pending.Push(root);
        while (pending.Count > 0)
        {
            var current = pending.Pop();
            string[] files; try { files = Directory.GetFiles(current); } catch { files = Array.Empty<string>(); }
            foreach (var file in files) yield return file;
            string[] dirs; try { dirs = Directory.GetDirectories(current); } catch { dirs = Array.Empty<string>(); }
            foreach (var dir in dirs)
            {
                try { if ((File.GetAttributes(dir) & FileAttributes.ReparsePoint) == 0) pending.Push(dir); } catch { }
            }
        }
    }
}