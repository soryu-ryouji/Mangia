using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Mangia.View;

public partial class MangaDetailView : UserControl
{
    public MangaDetailView(string mangaTitle, string coverPath, string folderPath)
    {
        InitializeComponent();

        var viewModel = new MangaDetailViewModel(mangaTitle, coverPath, folderPath);
        this.DataContext = viewModel;
    }
}

public class MangaDetailViewModel
{
    public string MangaTitle { get; }
    public string CoverPath { get; }
    public ObservableCollection<MangaChapter> Chapters { get; } = new();

    public MangaDetailViewModel(string mangaTitle, string coverPath, string folderPath)
    {
        MangaTitle = mangaTitle;
        CoverPath = coverPath;

        if (Directory.Exists(folderPath))
        {
            var dirs = new DirectoryInfo(folderPath).GetDirectories()
                .Where(d => !d.Name.StartsWith(".") && (d.Attributes & FileAttributes.Hidden) == 0);

            foreach (var chapterDir in dirs)
            {
                Chapters.Add(new MangaChapter
                {
                    Name = chapterDir.Name,
                    FolderPath = chapterDir.FullName
                });
            }
        }
    }
}

public class MangaChapter
{
    public string Name { get; set; }
    public string FolderPath { get; set; }
}