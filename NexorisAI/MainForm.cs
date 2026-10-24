using NexorisAI.Core;
namespace NexorisAI;
public sealed class MainForm : Form
{
    private readonly TextBox output = new();
    private readonly Label status = new();
    private readonly FlowLayoutPanel buttons = new();
    private bool diagnosticRunning;

    public MainForm()
    {
        Text = "NEXORIS AI — Computer Diagnostics"; Width = 1120; Height = 760; MinimumSize = new Size(820, 560);
        StartPosition = FormStartPosition.CenterScreen; BackColor = Color.FromArgb(16, 23, 36); ForeColor = Color.Gainsboro;
        var title = new Label { Text = "NEXORIS AI", Font = new Font("Segoe UI", 20, FontStyle.Bold), AutoSize = true, ForeColor = Color.FromArgb(111, 214, 199), Location = new Point(22, 18) };
        var subtitle = new Label { Text = "Offline-first diagnostics • read-only by default", AutoSize = true, ForeColor = Color.LightGray, Location = new Point(25, 62) };
        buttons.Location = new Point(22, 98); buttons.Height = 48; buttons.Width = 1050; buttons.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; buttons.WrapContents = false; buttons.BackColor = BackColor;
        AddButton(buttons, "System overview", () => ShowResult(SystemSnapshot.Capture()));
        AddButton(buttons, "Hardware inventory", () => ShowResult(HardwareInventory.Capture()));
        AddButton(buttons, "Device health", () => ShowResult(DeviceHealthAssessment.Capture()));
        AddAsyncButton(buttons, "Full diagnostic", FullDiagnosticReport.Capture);
        AddButton(buttons, "Installed apps", () => ShowResult(InstalledAppReader.Capture()));
        AddButton(buttons, "Cleanup preview", () => ShowResult(CleanupScanner.Preview()));
        AddButton(buttons, "Save report", SaveReport);
        output.Location = new Point(22, 158); output.Size = new Size(1050, 500); output.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        output.Multiline = true; output.ReadOnly = true; output.WordWrap = false; output.ScrollBars = ScrollBars.Both; output.Font = new Font("Consolas", 10);
        output.BackColor = Color.FromArgb(9, 14, 24); output.ForeColor = Color.Gainsboro;
        status.Text = "Ready • no changes made"; status.AutoSize = true; status.Location = new Point(24, 675); status.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        Controls.AddRange(new Control[] { title, subtitle, buttons, output, status });
        ShowResult("Welcome to NEXORIS AI.\r\n\r\nChoose a read-only diagnostic above.\r\n\r\nCompatibility depends on Windows version, drivers, firmware and hardware interfaces. A missing reading is not proof that a component is absent or faulty.\r\n\r\nCleanup previews candidates only; it does not delete files.");
    }

    private static Button CreateButton(string text) => new()
    {
        Text = text, AutoSize = true, Height = 34, FlatStyle = FlatStyle.Flat,
        BackColor = Color.FromArgb(35, 49, 68), ForeColor = Color.White,
        Margin = new Padding(0, 3, 8, 3), Padding = new Padding(8, 0, 8, 0)
    };

    private static void AddButton(FlowLayoutPanel panel, string text, Action action)
    {
        var button = CreateButton(text);
        button.Click += (_, _) => action();
        panel.Controls.Add(button);
    }

    private void AddAsyncButton(FlowLayoutPanel panel, string text, Func<string> capture)
    {
        var button = CreateButton(text);
        button.Click += async (_, _) =>
        {
            if (diagnosticRunning) return;
            diagnosticRunning = true;
            foreach (Control control in buttons.Controls) control.Enabled = false;
            UseWaitCursor = true;
            status.Text = "Full diagnostic running in background • no system changes";
            output.Text = "Collecting system overview, hardware inventory, and Windows-reported device health...\r\nPlease wait. The application should remain responsive.";
            try
            {
                var result = await Task.Run(capture);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                output.Text = "Full diagnostic could not be completed.\r\nError type: " + ex.GetType().Name + "\r\nNo system changes were made.";
                status.Text = "Diagnostic failed • no system changes";
            }
            finally
            {
                UseWaitCursor = false;
                foreach (Control control in buttons.Controls) control.Enabled = true;
                diagnosticRunning = false;
            }
        };
        panel.Controls.Add(button);
    }

    private void ShowResult(string text) { output.Text = text; status.Text = "Completed " + DateTime.Now.ToString("HH:mm:ss") + " • read-only • no system changes"; }

    private void SaveReport()
    {
        using var dialog = new SaveFileDialog { Filter = "Text report (*.txt)|*.txt", FileName = "NEXORIS-report-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt", Title = "Save diagnostic report" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try { File.WriteAllText(dialog.FileName, output.Text); status.Text = "Report saved to selected path."; }
        catch (Exception ex) { MessageBox.Show(this, "Could not save report: " + ex.Message, "NEXORIS AI", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}