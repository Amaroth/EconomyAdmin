using EconomyAdmin.Core;
using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;
using System;
using System.Security;

namespace EconomyAdmin.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        Config conf = Config.Instance;
        public LoginWindow()
        {
            Connector.Instance.ValidateVersion();
            InitializeComponent();
        }

        internal void Load()
        {
            Title = string.Format("EconomyAdmin {0} - Přihlášení", conf.version);
            loginBox.Text = Utilities.ToInsecureString(conf.userLogin);
            passBox.Password = Utilities.ToInsecureString(conf.userPassword);
            saveCredsBox.IsChecked = conf.saveCredentials;
        }

        private void loginButt_Click(object sender, RoutedEventArgs e)
        {
            conf.userLogin = Utilities.ToSecureString(loginBox.Text);
            conf.userPassword = Utilities.ToSecureString(passBox.Password);
            conf.companyID = companyBox.SelectedIndex;
            conf.saveCredentials = (bool)saveCredsBox.IsChecked;
            conf.SaveUserSettings();
        }
    }
}
