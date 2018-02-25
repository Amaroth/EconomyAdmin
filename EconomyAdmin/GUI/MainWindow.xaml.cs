using EconomyAdmin.Core;
using System.Windows;

namespace EconomyAdmin.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Config conf = Config.Instance;
        Connector conn = Connector.Instance;

        public MainWindow()
        {
            InitializeComponent();
            ReloadAll();
        }

        private void ReloadAll()
        {
            Title = string.Format("EconomyAdmin {0}", conf.version);
        }
    }
}
