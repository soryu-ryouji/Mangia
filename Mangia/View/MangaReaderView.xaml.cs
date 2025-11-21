using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Mangia.View;

public partial class MangaReaderView : UserControl
{
    private readonly List<string> _pages = new();
    private int _pairIndex = 0; // each pair shows two pages: left = pairIndex*2, right = left+1

    public string ChapterFolder { get; private set; } = string.Empty;

    private readonly DispatcherTimer _hideToolbarTimer;
    private const double ToolbarShowThresholdPx = 100.0; // distance from bottom to trigger toolbar show
    private const int ToolbarAutoHideMs = 500;

    public MangaReaderView()
    {
        InitializeComponent();
        Loaded += MangaReaderView_Loaded;
        KeyDown += MangaReaderView_KeyDown;

        // Timer to auto-hide toolbar after inactivity
        _hideToolbarTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(ToolbarAutoHideMs)
        };
        _hideToolbarTimer.Tick += (s, e) => { HideToolbar(); _hideToolbarTimer.Stop(); };

        // Recalculate constraints when the pages area resizes
        PagesGrid.SizeChanged += PagesGrid_SizeChanged;
    }

    public MangaReaderView(string chapterFolder) : this()
    {
        LoadChapter(chapterFolder);
    }

    private void MangaReaderView_Loaded(object? sender, RoutedEventArgs e)
    {
        // focus to receive keyboard input
        Focus();
    }

    public void LoadChapter(string chapterFolder)
    {
        ChapterFolder = chapterFolder ?? string.Empty;
        _pages.Clear();
        _pairIndex = 0;

        if (Directory.Exists(ChapterFolder))
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
            _pages.AddRange(Directory.EnumerateFiles(ChapterFolder)
                .Where(f => allowed.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase));
        }

        DisplayPair();
    }

    private void PagesGrid_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        // Update image constraints so Stretch="Uniform" will respect container size and keep aspect ratio
        UpdateImageConstraints();
    }

    private void DisplayPair()
    {
        int leftIndex = _pairIndex * 2;
        int rightIndex = leftIndex + 1;

        SetImageFromPath(LeftImage, leftIndex < _pages.Count ? _pages[leftIndex] : null);
        SetImageFromPath(RightImage, rightIndex < _pages.Count ? _pages[rightIndex] : null);

        // Hide right column if there is no right page
        RightImage.Visibility = (rightIndex < _pages.Count) ? Visibility.Visible : Visibility.Collapsed;

        // Make sure images fit current container
        UpdateImageConstraints();

        UpdateStatus();
    }

    private static void SetImageFromPath(Image target, string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            target.Source = null;
            return;
        }

        try
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad; // avoid file lock
            bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmp.EndInit();
            bmp.Freeze();
            target.Source = bmp;
        }
        catch
        {
            target.Source = null;
        }
    }

    private void UpdateImageConstraints()
    {
        // Ensure PagesGrid has measured size
        if (PagesGrid == null) return;
        double totalW = PagesGrid.ActualWidth;
        double totalH = PagesGrid.ActualHeight;
        if (double.IsNaN(totalW) || double.IsNaN(totalH) || totalW <= 0 || totalH <= 0) return;

        // Available area per page (leave a small padding)
        double padding = 16.0;
        double availWidth = Math.Max(1.0, (totalW / 2.0) - padding);
        double availHeight = Math.Max(1.0, totalH - padding);

        // Set constraints so Image.Stretch = Uniform will preserve aspect ratio and never exceed available space.
        LeftImage.MaxWidth = availWidth;
        LeftImage.MaxHeight = availHeight;

        RightImage.MaxWidth = availWidth;
        RightImage.MaxHeight = availHeight;

        // Also ensure scroll viewers do not auto-scale images beyond constraints
        LeftScroll.UpdateLayout();
        RightScroll.UpdateLayout();
    }

    private void PrevPair()
    {
        if (_pairIndex > 0)
        {
            _pairIndex--;
            DisplayPair();
        }
    }

    private void NextPair()
    {
        if ((_pairIndex + 1) * 2 < _pages.Count)
        {
            _pairIndex++;
            DisplayPair();
        }
    }

    private void UpdateStatus()
    {
        int left = _pairIndex * 2 + 1;
        int right = Math.Min(_pairIndex * 2 + 2, _pages.Count);
        if (_pages.Count == 0)
        {
            StatusText.Text = "No pages";
        }
        else if (left == right)
        {
            StatusText.Text = $"Page {left}/{_pages.Count}";
        }
        else
        {
            StatusText.Text = $"Pages {left}-{right}/{_pages.Count}";
        }
    }

    private void FitButton_Click(object sender, RoutedEventArgs e)
    {
        // Toggle behavior: if Viewbox is used, Fit toggles between Uniform and UniformToFill on images
        if (LeftImage.Stretch == System.Windows.Media.Stretch.Uniform)
        {
            LeftImage.Stretch = System.Windows.Media.Stretch.UniformToFill;
            RightImage.Stretch = System.Windows.Media.Stretch.UniformToFill;
        }
        else
        {
            LeftImage.Stretch = System.Windows.Media.Stretch.Uniform;
            RightImage.Stretch = System.Windows.Media.Stretch.Uniform;
        }

        // Re-apply constraints so UniformToFill behaves reasonably
        UpdateImageConstraints();
    }

    // Centralized click handling: clicking left half => previous pair, right half => next pair.
    private void OnViewerMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        // Get position relative to PagesGrid
        var pos = e.GetPosition(PagesGrid);

        // If click occurred outside PagesGrid (e.g., toolbar), ignore.
        if (pos.X < 0 || pos.Y < 0 || pos.X > PagesGrid.ActualWidth || pos.Y > PagesGrid.ActualHeight)
            return;

        // Decide left or right half
        if (PagesGrid.ActualWidth <= 0) return;
        if (pos.X < PagesGrid.ActualWidth / 2.0)
            PrevPair();
        else
            NextPair();
    }

    // Show/hide toolbar logic: user asked to only show toolbar when mouse near bottom.
    private void RootGrid_MouseMove(object sender, MouseEventArgs e)
    {
        // position relative to entire control
        var pos = e.GetPosition(this);
        if (double.IsNaN(ActualHeight) || ActualHeight <= 0) return;

        // Show toolbar when mouse is within threshold from bottom OR when mouse is over the toolbar itself
        bool nearBottom = pos.Y >= (ActualHeight - ToolbarShowThresholdPx);
        bool overToolbar = ToolbarPanel.IsMouseOver;

        if (nearBottom || overToolbar)
        {
            ShowToolbarTransient();
        }
        // else do nothing; timer will hide toolbar after inactivity
    }

    private void RootGrid_MouseLeave(object sender, MouseEventArgs e)
    {
        // start quick hide when mouse leaves the control entirely
        _hideToolbarTimer.Interval = TimeSpan.FromMilliseconds(300);
        _hideToolbarTimer.Start();
    }

    private void ShowToolbarTransient()
    {
        // Make toolbar visible and restart hide timer
        if (ToolbarPanel.Visibility != Visibility.Visible)
            ToolbarPanel.Visibility = Visibility.Visible;

        // Reset interval back to default and restart timer
        _hideToolbarTimer.Interval = TimeSpan.FromMilliseconds(ToolbarAutoHideMs);
        _hideToolbarTimer.Stop();
        _hideToolbarTimer.Start();
    }

    private void HideToolbar()
    {
        if (ToolbarPanel.Visibility != Visibility.Collapsed)
            ToolbarPanel.Visibility = Visibility.Collapsed;
    }

    // Keyboard navigation
    private void MangaReaderView_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Right || e.Key == Key.Space)
        {
            NextPair();
            e.Handled = true;
        }
        else if (e.Key == Key.Left)
        {
            PrevPair();
            e.Handled = true;
        }
        else if (e.Key == Key.Home)
        {
            _pairIndex = 0;
            DisplayPair();
            e.Handled = true;
        }
        else if (e.Key == Key.End)
        {
            _pairIndex = Math.Max(0, (_pages.Count - 1) / 2);
            DisplayPair();
            e.Handled = true;
        }
    }
}