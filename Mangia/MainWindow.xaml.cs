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

namespace Mangia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainMenu_Content.Content = new LibraryView();
        }

        private void Library_Click(object sender, RoutedEventArgs e)
        {
            MainMenu_Content.Content = new LibraryView();
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            MainMenu_Content.Content = new ConfigView();
        }
    }
}