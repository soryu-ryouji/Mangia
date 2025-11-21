using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mangia.View;

public partial class MangaDetailView : UserControl
{
    public event EventHandler<ChapterInfo>? ChapterSelected;

    public MangaDetailView(MangaInfo manga)
    {
        InitializeComponent();

        var viewModel = new MangaDetailViewModel(manga);
        this.DataContext = viewModel;
    }

    private void ChaptersListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ChaptersListBox.SelectedItem is ChapterInfo chapter)
        {
            ChapterSelected?.Invoke(this, chapter);
        }
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