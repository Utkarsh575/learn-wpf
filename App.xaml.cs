using System;
using System.IO;
using System.Windows;

namespace TestAvalonia;

public partial class App : Application
{
    private readonly string _logPath;

    public App()
    {
        _logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TestAvalonia", "log.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(_logPath) ?? string.Empty);

        DispatcherUnhandledException += App_DispatcherUnhandledException;
    }

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            var msg = $"[{DateTime.UtcNow:O}] Unhandled exception: {e.Exception}\n";
            File.AppendAllText(_logPath, msg);
        }
        catch { }

        // Show a simple message and prevent crash so the user can see the UI
        try
        {
            MessageBox.Show($"Unhandled exception: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch { }

        e.Handled = true; // Prevent app from crashing
    }
}