using EconomyAdmin.Core;
using System.Collections.Generic;
using System.Windows;

namespace EconomyAdmin.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        internal void Load()
        {
            Title = string.Format("EconomyAdmin {0} - Přihlášení", Config.Instance.version);
            companyBox.Items.Add("test");
            companyBox.Items.Add("test2");
        }
    }
}
