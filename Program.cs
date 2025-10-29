using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

class HourlyReminderTray : ApplicationContext
{
    private NotifyIcon trayIcon;
    private System.Threading.Timer reminderTimer;

    public HourlyReminderTray()
    {
        // Create system tray icon
        trayIcon = new NotifyIcon()
        {
            Icon = SystemIcons.Information,
            Visible = true,
            Text = "Code Push Reminder"
        };

        // Create context menu
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Show Next Reminder", null, ShowNextReminder);
        contextMenu.Items.Add("-");
        contextMenu.Items.Add("Exit", null, Exit);
        trayIcon.ContextMenuStrip = contextMenu;

        // Start the hourly reminder
        ScheduleNextReminder();

        trayIcon.ShowBalloonTip(3000, "Code Push Reminder",
            "Running in background. Right-click icon for options.",
            ToolTipIcon.Info);
    }

    private void ScheduleNextReminder()
    {
        DateTime now = DateTime.Now;
        DateTime nextHour = now.Date.AddHours(now.Hour + 1);
        TimeSpan delay = nextHour - now;

        reminderTimer?.Dispose();
        reminderTimer = new System.Threading.Timer(
            OnReminderTick,
            null,
            delay,
            System.Threading.Timeout.InfiniteTimeSpan
        );
    }

    private void OnReminderTick(object state)
    {
        // Show notification
        trayIcon.ShowBalloonTip(5000,
            "Code Push Reminder",
            "Don't forget to push the code!",
            ToolTipIcon.Warning);

        // Play sound
        System.Media.SystemSounds.Asterisk.Play();

        // Schedule next reminder
        ScheduleNextReminder();
    }

    private void ShowNextReminder(object sender, EventArgs e)
    {
        DateTime now = DateTime.Now;
        DateTime nextHour = now.Date.AddHours(now.Hour + 1);

        MessageBox.Show(
            $"Next reminder at: {nextHour:hh:mm tt}",
            "Code Push Reminder",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    private void Exit(object sender, EventArgs e)
    {
        reminderTimer?.Dispose();
        trayIcon.Visible = false;
        trayIcon.Dispose();
        Application.Exit();
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new HourlyReminderTray());
    }
}