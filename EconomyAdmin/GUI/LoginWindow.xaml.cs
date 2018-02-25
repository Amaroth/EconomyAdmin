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
        public LoginWindow()
        {
            Connector.Instance.ValidateVersion();
            InitializeComponent();
        }

        internal void Load()
        {
            Title = string.Format("EconomyAdmin {0} - Přihlášení", Config.Instance.version);
        }
    }
}
