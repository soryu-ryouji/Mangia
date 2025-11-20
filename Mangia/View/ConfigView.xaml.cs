using System;
using System.Collections.Generic;
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

namespace Mangia.View
{
    public partial class ConfigView : UserControl
    {
        public ConfigView()
        {
            InitializeComponent();

            LibraryPathPicker.Path = App.Config.LibraryPath;
            LibraryPathPicker.PathChanged += OnLibraryPathChanged;
        }

        private void OnLibraryPathChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            App.Config.LibraryPath = LibraryPathPicker.Path;
            App.Config.SaveConfig();
        }
    }
}
