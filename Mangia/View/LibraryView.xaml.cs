using System;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mangia.View
{
    public partial class LibraryView : UserControl
    {
        public ObservableCollection<LibraryFolder> FolderList { get; } = new ObservableCollection<LibraryFolder>();

        public event EventHandler<LibraryFolder>? ItemClicked;

        public LibraryView()
        {
            InitializeComponent();
            DataContext = this;
            LoadLibraryFolders();
        }

        private void LoadLibraryFolders()
        {
            string libPath = App.Config.LibraryPath;
            if (!Directory.Exists(libPath)) return;

            var dirInfos = new DirectoryInfo(libPath)
                .GetDirectories()
                .Where(di => !di.Name.StartsWith(".") &&
                             (di.Attributes & FileAttributes.Hidden) == 0);

            foreach (var dir in dirInfos)
            {
                var cover = dir.GetFiles()
                    .Where(f =>
                        !f.Name.StartsWith(".") &&
                        (f.Attributes & FileAttributes.Hidden) == 0 &&
                        (f.Name.Equals("cover.jpg", StringComparison.OrdinalIgnoreCase) ||
                         f.Name.Equals("cover.png", StringComparison.OrdinalIgnoreCase)))
                    .FirstOrDefault();

                if (cover == null)
                {
                    cover = dir.GetFiles()
                        .Where(f =>
                            !f.Name.StartsWith(".") &&
                            (f.Attributes & FileAttributes.Hidden) == 0 &&
                            f.Name.StartsWith("cover", StringComparison.OrdinalIgnoreCase) &&
                            (f.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                             f.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase)))
                        .FirstOrDefault();
                }

                if (cover != null)
                {
                    FolderList.Add(new LibraryFolder
                    {
                        FolderPath = dir.FullName,
                        FolderName = dir.Name,
                        CoverPath = cover.FullName
                    });
                }
            }
        }

        private void OnLibraryItemClick(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.DataContext is LibraryFolder folder)
            {
                ItemClicked?.Invoke(this, folder); // 通知 MainWindow
            }
        }
    }

    public class LibraryFolder
    {
        public string FolderPath { get; set; } = string.Empty;
        public string FolderName { get; set; } = string.Empty;
        public string CoverPath { get; set; } = string.Empty;
    }
}