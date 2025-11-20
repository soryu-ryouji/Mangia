using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace Mangia
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AppConfig Config { get; private set; } = new AppConfig();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Config = AppConfig.LoadDefaultConfig();
        }
    }

}
