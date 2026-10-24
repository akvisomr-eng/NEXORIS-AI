using Microsoft.Win32;
using System.Management;
using System.Runtime.InteropServices;
namespace NexorisAI.Core;
public static class HardwareInventory
{
    public static string Capture()
    {
        var lines = new List<string> { "HARDWARE DISCOVERY (best effort; read-only)" };
        AddWmi(lines, "Motherboard / Baseboard", "SELECT Manufacturer, Product, Version FROM Win32_BaseBoard", o => Join(o, "Manufacturer", "Product", "Version"));
        AddWmi(lines, "BIOS / Firmware", "SELECT Manufacturer, SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS", o => Join(o, "Manufacturer", "SMBIOSBIOSVersion", "ReleaseDate"));
        AddWmi(lines, "Processor", "SELECT Name, Manufacturer, NumberOfCores, NumberOfLogicalProcessors FROM Win32_Processor", o => Join(o, "Name", "Manufacturer", "NumberOfCores", "NumberOfLogicalProcessors"));
        AddWmi(lines, "Graphics adapters", "SELECT Name, DriverVersion, AdapterRAM FROM Win32_VideoController", o => Join(o, "Name", "DriverVersion", "AdapterRAM"));
        AddWmi(lines, "Memory modules", "SELECT Manufacturer, PartNumber, Capacity, Speed, ConfiguredClockSpeed FROM Win32_PhysicalMemory", o => Join(o, "Manufacturer", "PartNumber", "Capacity", "Speed", "ConfiguredClockSpeed"));
        AddWmi(lines, "Storage devices", "SELECT Model, InterfaceType, MediaType, Status FROM Win32_DiskDrive", o => Join(o, "Model", "InterfaceType", "MediaType", "Status"));
        AddWmi(lines, "Network adapters", "SELECT Name, Manufacturer, NetEnabled, Speed FROM Win32_NetworkAdapter WHERE PhysicalAdapter = TRUE", o => Join(o, "Name", "Manufacturer", "NetEnabled", "Speed"));
        AddWmi(lines, "Plug-and-play devices (first 100)", "SELECT Name, Manufacturer, PNPClass, Status, ConfigManagerErrorCode FROM Win32_PnPEntity", o => Join(o, "Name", "Manufacturer", "PNPClass", "Status", "ConfigManagerErrorCode"), 100);
        lines.Add("Windows build metadata: " + ReadWindowsBuild());
        lines.Add("Privacy: serial numbers and MAC addresses are intentionally omitted.");
        lines.Add("Limits: missing data can mean firmware/driver/provider does not expose it. Vendor-specific temperatures, fan speeds, EC registers and motherboard power rails require compatible drivers/interfaces or external instruments.");
        return string.Join(Environment.NewLine, lines);
    }
    private static void AddWmi(List<string> lines, string title, string query, Func<ManagementBaseObject, string> formatter, int limit = 30)
    {
        lines.Add(Environment.NewLine + title + ":");
        try
        {
            using var searcher = new ManagementObjectSearcher(query);
            using var results = searcher.Get();
            var count = 0;
            foreach (ManagementBaseObject item in results)
            {
                using (item)
                {
                    var value = formatter(item);
                    lines.Add("  - " + (string.IsNullOrWhiteSpace(value) ? "Reported; details unavailable" : value));
                    if (++count >= limit) { lines.Add("  - Output limited for readability."); break; }
                }
            }
            if (count == 0) lines.Add("  - No entries returned by this provider.");
        }
        catch (Exception ex) { lines.Add("  - Unavailable: " + ex.GetType().Name + " (provider, permission, or OS limitation)."); }
    }
    private static string Join(ManagementBaseObject o, params string[] keys)
    {
        var parts = new List<string>();
        foreach (var key in keys)
        {
            object? raw;
            try { raw = o[key]; } catch { raw = null; }
            if (raw is null) continue;
            var value = Convert.ToString(raw) ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(value)) parts.Add(key + ": " + value.Trim());
        }
        return string.Join(" | ", parts);
    }
    private static string ReadWindowsBuild()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var product = key?.GetValue("ProductName")?.ToString() ?? "Windows (name unavailable)";
            var display = key?.GetValue("DisplayVersion")?.ToString() ?? key?.GetValue("ReleaseId")?.ToString() ?? "not exposed";
            var build = key?.GetValue("CurrentBuildNumber")?.ToString() ?? key?.GetValue("CurrentBuild")?.ToString() ?? "unknown";
            var ubr = key?.GetValue("UBR")?.ToString() ?? "?";
            return product + "; version: " + display + "; build: " + build + "." + ubr + "; kernel: " + Environment.OSVersion.Version + "; process architecture: " + RuntimeInformation.ProcessArchitecture + "; OS architecture: " + RuntimeInformation.OSArchitecture;
        }
        catch (Exception ex) { return "Registry metadata unavailable (" + ex.GetType().Name + "); kernel: " + Environment.OSVersion.Version; }
    }
}