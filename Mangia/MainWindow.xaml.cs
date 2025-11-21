using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mangia.View;

namespace Mangia;

public partial class MainWindow : Window
{
    private LibraryView? _libraryView;
    private MangaDetailView? _mangaDetailView;
    public MainWindow()
    {
        InitializeComponent();

        SwitchToLibraryView();
    }

    public void SwitchToLibraryView()
    {
        if (_libraryView == null)
        {
            _libraryView = new LibraryView();
            _libraryView.ItemClicked += LibraryView_ItemClicked;
        }

        MainMenuContent.Content = _libraryView;
    }

    private void Library_Click(object sender, RoutedEventArgs e)
    {
        SwitchToLibraryView();
    }

    private void Config_Click(object sender, RoutedEventArgs e)
    {
        MainMenuContent.Content = new ConfigView();
    }

    private void LibraryView_ItemClicked(object? sender, MangaInfo manga)
    {
        var detailView = new MangaDetailView(manga);

        // unsubscribe previous if any, then subscribe to new
        if (_mangaDetailView != null)
            _mangaDetailView.ChapterSelected -= MangaDetailView_ChapterSelected;

        _mangaDetailView = detailView;
        _mangaDetailView.ChapterSelected += MangaDetailView_ChapterSelected;

        MainMenuContent.Content = detailView;
    }

    private void MangaDetailView_ChapterSelected(object? sender, ChapterInfo chapter)
    {
        // open reader for selected chapter folder
        var reader = new MangaReaderView(chapter.Url);
        MainMenuContent.Content = reader;
    }
}