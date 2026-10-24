using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NexorisAI.Core;

public static class SystemSnapshot
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private sealed class MemoryStatusEx
    {
        public uint Length = (uint)Marshal.SizeOf<MemoryStatusEx>();
        public uint MemoryLoad;
        public ulong TotalPhysical;
        public ulong AvailablePhysical;
        public ulong TotalPageFile;
        public ulong AvailablePageFile;
        public ulong TotalVirtual;
        public ulong AvailableVirtual;
        public ulong AvailableExtendedVirtual;
    }

    [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GlobalMemoryStatusEx")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx([In, Out] MemoryStatusEx buffer);

    public static string Capture()
    {
        var lines = new List<string>
        {
            "SYSTEM OVERVIEW (read-only)",
            "OS description: " + RuntimeInformation.OSDescription,
            "OS architecture: " + RuntimeInformation.OSArchitecture,
            "Process architecture: " + RuntimeInformation.ProcessArchitecture,
            ".NET runtime: " + RuntimeInformation.FrameworkDescription,
            "Logical CPU count: " + Environment.ProcessorCount,
            "System uptime: " + TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"d\.hh\:mm\:ss"),
            "Process working set: " + (Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024)) + " MiB"
        };
        try
        {
            var memory = new MemoryStatusEx();
            if (GlobalMemoryStatusEx(memory))
            {
                lines.Add("Physical RAM total: " + ToMiB(memory.TotalPhysical) + " MiB");
                lines.Add("Physical RAM available: " + ToMiB(memory.AvailablePhysical) + " MiB");
                lines.Add("Physical memory load: " + memory.MemoryLoad + "%");
                lines.Add("Page file available: " + ToMiB(memory.AvailablePageFile) + " MiB");
            }
            else lines.Add("Physical memory details unavailable (Windows API returned false).");
        }
        catch (Exception ex) { lines.Add("Physical memory details unavailable (" + ex.GetType().Name + ")."); }
        lines.Add("No changes were made to the system.");
        return string.Join(Environment.NewLine, lines);
    }

    private static ulong ToMiB(ulong bytes) => bytes / (1024UL * 1024UL);
}