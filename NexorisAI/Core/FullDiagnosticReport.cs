namespace NexorisAI.Core;

/// <summary>
/// Combines key read-only diagnostics into one local report. Each section handles
/// provider failures independently so one unavailable subsystem does not discard the others.
/// </summary>
public static class FullDiagnosticReport
{
    public static string Capture()
    {
        var sections = new[]
        {
            ("SYSTEM OVERVIEW", (Func<string>)SystemSnapshot.Capture),
            ("HARDWARE INVENTORY", HardwareInventory.Capture),
            ("DEVICE HEALTH ASSESSMENT", DeviceHealthAssessment.Capture)
        };

        var report = new List<string>
        {
            "NEXORIS AI — FULL DIAGNOSTIC REPORT",
            "Generated locally: " + DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"),
            "Mode: read-only; no system changes are made.",
            "Privacy: this report may include device model names and Windows-reported status details. Review it before sharing.",
            new string('=', 78)
        };

        foreach (var (title, capture) in sections)
        {
            report.Add("");
            report.Add("## " + title);
            report.Add(new string('-', 78));
            try
            {
                report.Add(capture());
            }
            catch (Exception ex)
            {
                report.Add("Section unavailable: " + ex.GetType().Name);
                report.Add("No changes were made.");
            }
        }

        report.Add("");
        report.Add(new string('=', 78));
        report.Add("Important: diagnostic results are best-effort indicators, not proof of physical hardware condition.");
        return string.Join(Environment.NewLine, report);
    }
}
