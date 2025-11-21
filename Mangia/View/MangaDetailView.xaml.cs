using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Mangia.View;

public partial class MangaDetailView : UserControl
{
    public MangaDetailView(MangaInfo manga)
    {
        InitializeComponent();

        var viewModel = new MangaDetailViewModel(manga);
        this.DataContext = viewModel;
    }
}

public class MangaDetailViewModel
{
    public string MangaTitle { get; private set; }
    public string CoverPath { get; private set; }
    public ObservableCollection<ChapterInfo> ChapterInfos { get; } = new();

    public MangaDetailViewModel(MangaInfo info)
    {
        MangaTitle = info.Title;
        CoverPath = info.CoverUrl;
        ChapterInfos = new(info.ChapterInfos);
    }
}
