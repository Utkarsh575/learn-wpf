using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;

namespace TestAvalonia;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _sidebarCollapsed = false;
    private const double SidebarExpandedWidth = 240;
    private const double SidebarCollapsedWidth = 48;

    private bool _isSidebarCollapsed = false;
    public bool IsSidebarCollapsed
    {
        get => _isSidebarCollapsed;
        set
        {
            _isSidebarCollapsed = value;
            OnPropertyChanged(nameof(IsSidebarCollapsed));
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ObservableCollection<Workflow> Workflows { get; set; } = new();

    public MainWindow()
    {
        try
        {
            InitializeComponent();

            // Sample data
            var sampleWorkflow = new Workflow { Name = "Sample Workflow" };
            var task1 = new TaskItem { Name = "Task 1", Progress = 0.5 };
            task1.Steps.Add(new Step { Name = "Step 1.1", Url = "https://www.google.com", Progress = 0.8 });
            task1.Steps.Add(new Step { Name = "Step 1.2", Url = "https://www.github.com", Progress = 0.3 });
            var task2 = new TaskItem { Name = "Task 2", Progress = 0.2 };
            task2.Steps.Add(new Step { Name = "Step 2.1", Url = "https://www.jira.com", Progress = 0.1 });
            sampleWorkflow.Tasks.Add(task1);
            sampleWorkflow.Tasks.Add(task2);
            Workflows.Add(sampleWorkflow);

            WorkflowTreeView.ItemsSource = Workflows;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing app: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // Do not re-throw - show error and allow the app to continue so user can see the UI
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Kick off initialization but don't crash the UI if WebView2 fails
        _ = InitializeWebViewsAsync();
    }

    private async Task InitializeWebViewsAsync()
    {
        try
        {
            if (MainView != null) await MainView.EnsureCoreWebView2Async();
        }
        catch (Exception ex)
        {
            // Silent fail, but log
            System.Diagnostics.Debug.WriteLine($"WebView2 init failed: {ex.Message}");
        }
    }

    private void ToggleSidebarButton_Click(object sender, RoutedEventArgs e)
    {
        if (_sidebarCollapsed)
        {
            // Expand with animation
            var expandStoryboard = (Storyboard)FindResource("ExpandSidebar");
            expandStoryboard.Begin();
            _sidebarCollapsed = false;
            IsSidebarCollapsed = false;
        }
        else
        {
            // Collapse with animation
            var collapseStoryboard = (Storyboard)FindResource("CollapseSidebar");
            collapseStoryboard.Begin();
            _sidebarCollapsed = true;
            IsSidebarCollapsed = true;
        }
    }

    private void WorkflowTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        try
        {
            if (e.NewValue == null) return;
            if (e.NewValue is Step step)
            {
                // load the selected step into the single main view
                LoadUrlInView(MainView, step.Url);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading step: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenAllTasksButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // For now open the first available step in the main view
            var first = Workflows.SelectMany(w => w.Tasks).SelectMany(t => t.Steps).FirstOrDefault();
            if (first != null)
            {
                LoadUrlInView(MainView, first.Url);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error opening tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void LoadUrlInView(Microsoft.Web.WebView2.Wpf.WebView2? view, string url)
    {
        if (view == null)
            return;

        try
        {
            // Ensure WebView2 core is available before navigating
            if (view.CoreWebView2 == null)
            {
                try
                {
                    await view.EnsureCoreWebView2Async();
                }
                catch (Exception initEx)
                {
                    System.Diagnostics.Debug.WriteLine($"EnsureCoreWebView2Async failed: {initEx.Message}");
                }
            }

            if (view.CoreWebView2 != null)
            {
                view.CoreWebView2.Navigate(url);
            }
            else
            {
                // Fallback to setting Source if CoreWebView2 isn't available
                view.Source = new Uri(url);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading URL {url}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            try
            {
                view.Source = new Uri(url);
            }
            catch { }
        }
    }

    private void GoButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var url = UrlTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(url)) return;

            // normalize
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;

            LoadUrlInView(MainView, url);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error navigating: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MarkStepCompleteButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (WorkflowTreeView.SelectedItem is Step s)
            {
                s.IsCompleted = true;
                s.Progress = 1.0;
                WorkflowTreeView.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Please select a step to mark complete.", "Mark Step", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to mark step complete: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MarkTaskCompleteButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (WorkflowTreeView.SelectedItem is TaskItem t)
            {
                t.IsCompleted = true;
                t.Progress = 1.0;
                foreach (var st in t.Steps)
                {
                    st.IsCompleted = true;
                    st.Progress = 1.0;
                }
                WorkflowTreeView.Items.Refresh();
            }
            else if (WorkflowTreeView.SelectedItem is Step s)
            {
                // If a step is selected, mark its parent task completed
                var parentTask = Workflows.SelectMany(w => w.Tasks).FirstOrDefault(task => task.Steps.Contains(s));
                if (parentTask != null)
                {
                    parentTask.IsCompleted = true;
                    parentTask.Progress = 1.0;
                    foreach (var st in parentTask.Steps)
                    {
                        st.IsCompleted = true;
                        st.Progress = 1.0;
                    }
                    WorkflowTreeView.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Please select a task or a step (to mark its task complete).", "Mark Task", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to mark task complete: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadWorkflowButton_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog();
        dlg.Filter = "Workflow files (*.json)|*.json|All files|*.*";
        if (dlg.ShowDialog() == true)
        {
            if (string.IsNullOrEmpty(dlg.FileName)) return;
            try
            {
                var json = File.ReadAllText(dlg.FileName);
                var workflows = JsonSerializer.Deserialize<ObservableCollection<Workflow>>(json);
                if (workflows != null)
                {
                    Workflows.Clear();
                    foreach (var w in workflows)
                        Workflows.Add(w);
                }
                MessageBox.Show("Workflow loaded.", "Load Workflow", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load workflow: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void CreateWorkflowButton_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog();
        dlg.Filter = "Workflow files (*.json)|*.json|All files|*.*";
        dlg.DefaultExt = "json";
        if (dlg.ShowDialog() == true)
        {
            if (string.IsNullOrEmpty(dlg.FileName)) return;
            try
            {
                var json = JsonSerializer.Serialize(Workflows, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dlg.FileName, json);
                MessageBox.Show("Workflow saved.", "Create Workflow", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save workflow: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ShareButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var json = JsonSerializer.Serialize(Workflows, new JsonSerializerOptions { WriteIndented = true });
            Clipboard.SetText(json);
            MessageBox.Show("Workflow JSON copied to clipboard.", "Share Workflow", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to share workflow: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
#pragma warning restore CS8602, CS1998