using Microsoft.Win32;
using System.Management;
using System.Runtime.InteropServices;

namespace NexorisAI.Core;

/// <summary>Best-effort, read-only hardware discovery. WMI and firmware data vary by device.</summary>
public static class HardwareInventory
{
    public static string Capture()
    {
        var lines = new List<string> { "HARDWARE DISCOVERY (best effort; read-only)" };
        AddWmi(lines, "Motherboard / Baseboard", "SELECT Manufacturer, Product, Version FROM Win32_BaseBoard", o => Join(o, "Manufacturer", "Product", "Version"));
        AddWmi(lines, "BIOS / Firmware", "SELECT Manufacturer, SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS", o => Join(o, "Manufacturer", "SMBIOSBIOSVersion", "ReleaseDate"));
        AddWmi(lines, "Processor", "SELECT Name, Manufacturer, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed FROM Win32_Processor", o => Join(o, "Name", "Manufacturer", "NumberOfCores", "NumberOfLogicalProcessors", "MaxClockSpeed"));
        AddWmi(lines, "Graphics adapters", "SELECT Name, DriverVersion, AdapterRAM, Status, VideoModeDescription FROM Win32_VideoController", o => Join(o, "Name", "DriverVersion", "AdapterRAM", "Status", "VideoModeDescription"));
        AddWmi(lines, "Memory modules", "SELECT Manufacturer, PartNumber, Capacity, Speed, ConfiguredClockSpeed, FormFactor FROM Win32_PhysicalMemory", o => Join(o, "Manufacturer", "PartNumber", "Capacity", "Speed", "ConfiguredClockSpeed", "FormFactor"));
        AddWmi(lines, "Storage devices", "SELECT Model, Manufacturer, InterfaceType, MediaType, Status, Size FROM Win32_DiskDrive", o => Join(o, "Model", "Manufacturer", "InterfaceType", "MediaType", "Status", "Size"));
        AddWmi(lines, "Logical volumes", "SELECT DeviceID, FileSystem, DriveType, Size, FreeSpace, VolumeName FROM Win32_LogicalDisk", o => Join(o, "DeviceID", "FileSystem", "DriveType", "Size", "FreeSpace", "VolumeName"));
        AddWmi(lines, "Physical network adapters", "SELECT Name, Manufacturer, NetEnabled, Speed, AdapterType FROM Win32_NetworkAdapter WHERE PhysicalAdapter = TRUE", o => Join(o, "Name", "Manufacturer", "NetEnabled", "Speed", "AdapterType"));
        AddWmi(lines, "Audio devices", "SELECT Name, Manufacturer, Status FROM Win32_SoundDevice", o => Join(o, "Name", "Manufacturer", "Status"));
        AddWmi(lines, "Battery status", "SELECT Name, BatteryStatus, EstimatedChargeRemaining, EstimatedRunTime, DesignCapacity, FullChargeCapacity FROM Win32_Battery", o => Join(o, "Name", "BatteryStatus", "EstimatedChargeRemaining", "EstimatedRunTime", "DesignCapacity", "FullChargeCapacity"));
        AddWmi(lines, "USB controllers", "SELECT Name, Manufacturer, Status FROM Win32_USBController", o => Join(o, "Name", "Manufacturer", "Status"));
        AddWmi(lines, "Monitors", "SELECT Name, MonitorManufacturer, MonitorType, PixelsPerXLogicalInch, PixelsPerYLogicalInch FROM Win32_DesktopMonitor", o => Join(o, "Name", "MonitorManufacturer", "MonitorType", "PixelsPerXLogicalInch", "PixelsPerYLogicalInch"));
        AddWmi(lines, "Printers", "SELECT Name, DriverName, PortName, PrinterStatus, WorkOffline FROM Win32_Printer", o => Join(o, "Name", "DriverName", "PortName", "PrinterStatus", "WorkOffline"));
        AddWmi(lines, "Plug-and-play devices (first 250)", "SELECT Name, Manufacturer, PNPClass, Status, ConfigManagerErrorCode FROM Win32_PnPEntity", o => Join(o, "Name", "Manufacturer", "PNPClass", "Status", "ConfigManagerErrorCode"), 250);
        lines.Add("Windows build metadata: " + ReadWindowsBuild());
        lines.Add("Privacy: device serial numbers, MAC addresses, and unique hardware IDs are intentionally omitted.");
        lines.Add("Important: WMI may omit devices or report stale/incomplete firmware data. Missing readings are unavailable, not proof of hardware failure.");
        lines.Add("Limits: vendor-specific temperatures, fan speeds, embedded-controller registers, SMART/NVMe health, and motherboard power rails require supported drivers/interfaces or compatible external instruments.");
        return string.Join(Environment.NewLine, lines);
    }

    private static void AddWmi(List<string> lines, string title, string query, Func<ManagementBaseObject, string> formatter, int limit = 50)
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
                    lines.Add("  - " + (string.IsNullOrWhiteSpace(value) ? "Entry reported; details unavailable" : value));
                    if (++count >= limit) { lines.Add("  - Output capped at " + limit + " entries."); break; }
                }
            }
            if (count == 0) lines.Add("  - No entries returned by this provider (not necessarily absent).");
        }
        catch (Exception ex)
        {
            lines.Add("  - Unavailable: " + ex.GetType().Name + " (provider, permission, or OS capability limitation).");
        }
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