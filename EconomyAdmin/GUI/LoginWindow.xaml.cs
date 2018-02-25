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
        Connector conn = Connector.Instance;
        Dictionary<string, int> companies;

        public LoginWindow()
        {
            conn.ValidateVersion();
            InitializeComponent();
        }

        internal void Load()
        {
            Title = string.Format("EconomyAdmin {0} - Přihlášení", conf.version);
            loginBox.Text = Utilities.ToInsecureString(conf.userLogin);
            passBox.Password = Utilities.ToInsecureString(conf.userPassword);
            saveCredsBox.IsChecked = conf.saveCredentials;
        }

        private void DisableCompanySection()
        {
            companyLabel.IsEnabled = false;
            companyBox.IsEnabled = false;
            saveCredsBox.IsEnabled = false;
            loginEcoButt.IsEnabled = false;
            companyBox.Items.Clear();
        }

        private void EnableCompanySection()
        {
            companyLabel.IsEnabled = true;
            companyBox.IsEnabled = true;
            saveCredsBox.IsEnabled = true;
            loginEcoButt.IsEnabled = true;
        }

        private void loginAuthButt_Click(object sender, RoutedEventArgs e)
        {
            DisableCompanySection();
            var errCode = conn.IsUserValid(Utilities.ToSecureString(loginBox.Text), Utilities.ToSecureString(passBox.Password));
            if (errCode == 0)
            {
                companies = conn.GetAvailableCompanies();
                foreach (var c in companies)
                    companyBox.Items.Add(c.Key);

                if (companyBox.Items.Count > 0)
                {
                    companyBox.SelectedIndex = 0;
                    EnableCompanySection();
                    notificationText.Text = "Přihlášení úspěšné. Zvol si společnost a pokračuj.";
                }
                else
                    MessageBox.Show("Přihlášení na WoW účet proběhlo úspěšně, avšak nemáš přístup do žádné existující společnosti.");
            }
            else if (errCode == 1)
                MessageBox.Show("Chybná kombinace loginu a hesla.");
            else if (errCode == 2)
                MessageBox.Show("Je nám líto, tvůj účet je aktuálně zabanován.");
            else if (errCode == 3)
                MessageBox.Show("Je nám líto, tvá IP adresa je aktuálně zabanována.");
            else if (errCode == 4)
                MessageBox.Show("Nepodařilo se ověřit přihlášení. Zkontroluj své připojení k internetu, firewall, případně to zkus znovu. Pokud vše selže, kontaktuj GMT.");
        }

        private void loginEcoButt_Click(object sender, RoutedEventArgs e)
        {
            conf.userLogin = Utilities.ToSecureString(loginBox.Text);
            conf.userPassword = Utilities.ToSecureString(passBox.Password);
            conf.saveCredentials = (bool)saveCredsBox.IsChecked;
            conf.SaveUserSettings();
            conn.CompanyID = companies[companyBox.SelectedItem.ToString()];
            var mwd = new MainWindow();
            mwd.Show();
            Close();
        }
    }
}
