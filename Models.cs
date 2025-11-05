using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestAvalonia;

public class Workflow : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private string _name = "New Workflow";
    public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

    public ObservableCollection<TaskItem> Tasks { get; set; } = new();

    protected void OnPropertyChanged([CallerMemberName] string? prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}

public class TaskItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private string _name = "New Task";
    public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

    private double _progress = 0.0;
    public double Progress { get => _progress; set { _progress = value; OnPropertyChanged(); } }

    private bool _isCompleted = false;
    public bool IsCompleted { get => _isCompleted; set { _isCompleted = value; OnPropertyChanged(); } }

    public ObservableCollection<Step> Steps { get; set; } = new();

    protected void OnPropertyChanged([CallerMemberName] string? prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}

public class Step : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private string _name = "New Step";
    public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

    private string _url = "https://www.google.com";
    public string Url { get => _url; set { _url = value; OnPropertyChanged(); } }

    private double _progress = 0.0;
    public double Progress { get => _progress; set { _progress = value; OnPropertyChanged(); } }

    private bool _isCompleted = false;
    public bool IsCompleted { get => _isCompleted; set { _isCompleted = value; OnPropertyChanged(); } }

    protected void OnPropertyChanged([CallerMemberName] string? prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}