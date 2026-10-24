using System.Diagnostics;
using System.Runtime.InteropServices;
namespace NexorisAI.Core;
public static class SystemSnapshot
{
    public static string Capture()
    {
        var lines = new List<string> {
            "SYSTEM OVERVIEW (read-only)",
            "OS description: " + RuntimeInformation.OSDescription,
            "OS architecture: " + RuntimeInformation.OSArchitecture,
            "Process architecture: " + RuntimeInformation.ProcessArchitecture,
            ".NET runtime: " + RuntimeInformation.FrameworkDescription,
            "Logical CPU count: " + Environment.ProcessorCount,
            "System uptime: " + TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"d\.hh\:mm\:ss"),
            "Process working set: " + (Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024)) + " MiB",
            "No changes were made to the system."
        };
        return string.Join(Environment.NewLine, lines);
    }
}