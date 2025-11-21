using System;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Mangia.View
{
    public partial class LibraryView : UserControl
    {
        public ObservableCollection<MangaInfo> MangaList { get; private set; } = new ();

        public event EventHandler<MangaInfo>? ItemClicked;

        public LibraryView()
        {
            InitializeComponent();
            DataContext = this;
            LoadLibraryFolders();
        }

        private void LoadLibraryFolders()
        {
            if (Directory.Exists(App.Config.LibraryPath) is false) return;

            var libDir = new DirectoryInfo(App.Config.LibraryPath);
            var mangaInfos = libDir.GetDirectories()
                .Where(dir => dir.Name.StartsWith('.') is false)
                .Where(dir => dir.Attributes.HasFlag(FileAttributes.Hidden) is false)
                .Select(dir => new MangaInfo(dir.Name, dir.FullName))
                .ToList();

            MangaList = new ObservableCollection<MangaInfo>(mangaInfos);
        }

        private void OnLibraryItemClick(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.DataContext is MangaInfo manga)
            {
                ItemClicked?.Invoke(this, manga); // 通知 MainWindow
            }
        }
    }
}