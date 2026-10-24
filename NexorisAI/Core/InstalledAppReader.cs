using Microsoft.Win32;
namespace NexorisAI.Core;
public static class InstalledAppReader
{
    private static readonly string[] Paths = {
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
        @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
    };
    public static string Capture()
    {
        var lines = new List<string> { "INSTALLED APPLICATIONS (inventory only; uninstall disabled)" };
        foreach (var hive in new[] { Registry.LocalMachine, Registry.CurrentUser })
        foreach (var path in Paths)
        {
            try
            {
                using var root = hive.OpenSubKey(path);
                if (root is null) continue;
                foreach (var name in root.GetSubKeyNames())
                {
                    try
                    {
                        using var app = root.OpenSubKey(name);
                        var displayName = app?.GetValue("DisplayName")?.ToString();
                        if (string.IsNullOrWhiteSpace(displayName)) continue;
                        var version = app?.GetValue("DisplayVersion")?.ToString() ?? "version unknown";
                        var publisher = app?.GetValue("Publisher")?.ToString() ?? "publisher unknown";
                        lines.Add("- " + displayName + " | " + version + " | " + publisher);
                    }
                    catch { }
                    if (lines.Count >= 1001) { lines.Add("Output capped at 1,000 applications."); return string.Join(Environment.NewLine, lines); }
                }
            }
            catch { lines.Add("- Some entries in " + path + " were unavailable."); }
        }
        if (lines.Count == 1) lines.Add("No application entries were returned.");
        lines.Add("This inventory never executes uninstall strings or changes installed software.");
        return string.Join(Environment.NewLine, lines);
    }
}