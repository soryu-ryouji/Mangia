using System.Windows;
using System.Windows.Controls;
using Ookii.Dialogs.Wpf;

namespace Mangia.Component;

public partial class PathPicker : UserControl
{
    public PathPicker()
    {
        InitializeComponent();
    }

    public static readonly RoutedEvent PathChangedEvent =
        EventManager.RegisterRoutedEvent(
            nameof(PathChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<string>),
            typeof(PathPicker)
        );

    public event RoutedPropertyChangedEventHandler<string> PathChanged
    {
        add => AddHandler(PathChangedEvent, value);
        remove => RemoveHandler(PathChangedEvent, value);
    }

    public static readonly DependencyProperty PathProperty =
        DependencyProperty.Register(
            nameof(Path),
            typeof(string),
            typeof(PathPicker),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnPathChanged));

    public string Path
    {
        get => (string)GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (PathPicker)d;
        var args = new RoutedPropertyChangedEventArgs<string>(
            (string)e.OldValue,
            (string)e.NewValue,
            PathChangedEvent);
        ctrl.RaiseEvent(args);
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(PathPicker),
            new PropertyMetadata("路径")
        );

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    private void OnBrowseClick(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog
        {
            Description = "请选择文件夹",
            UseDescriptionForTitle = true,
            SelectedPath = Path ?? "",
            ShowNewFolderButton = true
        };

        if (dialog.ShowDialog() == true)
        {
            Path = dialog.SelectedPath; // 此处会自动通过依赖属性通知 PathChanged
        }
    }
}