using System.Management;

namespace NexorisAI.Core;

/// <summary>
/// Read-only summary of Windows Plug-and-Play device status. This is an indicator,
/// not a hardware self-test; WMI providers may be incomplete or stale.
/// </summary>
public static class DeviceHealthAssessment
{
    public static string Capture()
    {
        var lines = new List<string>
        {
            "DEVICE HEALTH ASSESSMENT (read-only)",
            "Source: Win32_PnPEntity / ConfigManagerErrorCode",
            "Interpretation: Windows-reported device status, not a physical hardware test.",
            ""
        };
        var normal = 0;
        var attention = 0;
        var informational = 0;
        var unavailable = 0;
        var inspected = 0;
        var unreadable = 0;

        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT Name, PNPClass, ConfigManagerErrorCode FROM Win32_PnPEntity");
            using var results = searcher.Get();
            foreach (ManagementBaseObject device in results)
            {
                using (device)
                {
                    inspected++;
                    try
                    {
                        var name = ReadProperty(device, "Name");
                        var category = ReadProperty(device, "PNPClass");
                        var raw = ReadRawProperty(device, "ConfigManagerErrorCode");
                        if (raw is null || !int.TryParse(Convert.ToString(raw), out var code))
                        {
                            unavailable++;
                            continue;
                        }

                        if (code == 0) { normal++; continue; }
                        if (code == 45) { informational++; continue; }

                        attention++;
                        lines.Add($"[{Severity(code)}] {Safe(name)}" +
                                  (string.IsNullOrWhiteSpace(category) ? "" : $" | Class: {category}") +
                                  $" | Code {code}: {Describe(code)}");
                        lines.Add("  Suggested next step: " + Recommendation(code));
                        lines.Add("");
                    }
                    catch
                    {
                        // A malformed or inaccessible record should not discard other devices.
                        unreadable++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lines.Add("Assessment unavailable: " + ex.GetType().Name +
                      ". Check that Windows Management Instrumentation is available.");
            lines.Add("No changes were made.");
            return string.Join(Environment.NewLine, lines);
        }

        lines.Insert(4, $"Devices inspected: {inspected} | Normal: {normal} | Needs attention: {attention} | Disconnected: {informational} | Status unavailable: {unavailable} | Records unreadable: {unreadable}");
        if (attention == 0)
            lines.Add("No nonzero device status codes requiring attention were found in the readable records.");
        else
            lines.Add("Prioritize critical work devices, then check Device Manager and vendor-supplied driver/firmware information before making changes.");

        lines.Add("");
        lines.Add("A code may reflect a driver, resource, firmware, or connection issue; it does not by itself prove physical damage.");
        lines.Add("NEXORIS has not changed drivers, settings, services, or device state.");
        return string.Join(Environment.NewLine, lines);
    }

    private static object? ReadRawProperty(ManagementBaseObject device, string property)
    {
        try { return device[property]; }
        catch { return null; }
    }

    private static string? ReadProperty(ManagementBaseObject device, string property) =>
        Convert.ToString(ReadRawProperty(device, property))?.Trim();

    private static string Severity(int code) => code switch
    {
        10 or 12 or 29 or 43 => "HIGH",
        22 or 28 or 31 or 32 or 37 or 39 or 48 or 49 => "WARNING",
        _ => "CHECK"
    };

    private static string Describe(int code) => code switch
    {
        1 => "not configured correctly",
        3 => "driver may be corrupted or resources may be low",
        10 => "device cannot start",
        12 => "insufficient free resources",
        14 => "restart required",
        18 => "driver should be reinstalled",
        22 => "device is disabled",
        24 => "device absent, not working, or driver details incomplete",
        28 => "drivers are not installed",
        29 => "disabled by firmware",
        31 => "Windows cannot load the required driver",
        32 => "driver service is disabled",
        33 => "required resources cannot be determined",
        34 => "device settings cannot be determined",
        37 => "driver initialization failed",
        38 => "previous driver instance is still loaded",
        39 => "driver is missing or corrupted",
        40 => "driver service configuration cannot be accessed",
        41 => "driver loaded but device was not found",
        42 => "duplicate device detected",
        43 => "Windows stopped the device after it reported a problem",
        44 => "an application or service shut down the device",
        45 => "device is not currently connected",
        48 => "driver blocked due to a known compatibility issue",
        49 => "system registry is too large for the device configuration",
        _ => "Windows reported a nonzero or unclassified device status"
    };

    private static string Recommendation(int code) => code switch
    {
        10 or 43 => "Open Device Manager, record the device and code, then check the manufacturer's support guidance and event logs.",
        12 or 33 or 49 => "Review resource conflicts and system/Device Manager diagnostics; do not change BIOS settings without understanding the impact.",
        22 => "Confirm whether the device was intentionally disabled before considering any change.",
        28 or 18 or 31 or 37 or 39 or 48 => "Identify the exact model and obtain a compatible driver from Windows Update or the device manufacturer's official support page.",
        29 => "Check the device/firmware documentation; do not change firmware settings blindly.",
        45 => "Check the physical connection if the device is expected to be connected.",
        14 => "Save work and consider a normal Windows restart when convenient.",
        _ => "Open Device Manager for the exact device and review its Properties > General status and the manufacturer's documentation."
    };

    private static string Safe(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "Device name unavailable" : value.Replace("\r", " ").Replace("\n", " ");
}
