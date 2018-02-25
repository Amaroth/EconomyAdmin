using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EconomyAdmin.Core
{
    class Connector
    {
        private static Connector instance;
        Config conf = Config.Instance;

        SecureString connectionStringEconomy;
        MySqlConnection connectionEconomy;

        SecureString connectionStringAuth;
        MySqlConnection connectionAuth;

        private Connector()
        {
            connectionStringEconomy = Utilities.ToSecureString(string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}; SslMode=none;",
                conf.hostEconomy, conf.portEconomy, conf.databaseEconomy, Utilities.ToInsecureString(conf.sqlLoginEconomy), Utilities.ToInsecureString(conf.sqlPasswordEconomy)));
            connectionEconomy = new MySqlConnection(Utilities.ToInsecureString(connectionStringEconomy));

            connectionStringAuth = Utilities.ToSecureString(string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}; SslMode=none;",
                conf.hostAuth, conf.portAuth, conf.databaseAuth, Utilities.ToInsecureString(conf.sqlLoginAuth), Utilities.ToInsecureString(conf.sqlPasswordAuth)));
            connectionAuth = new MySqlConnection(Utilities.ToInsecureString(connectionStringAuth));
        }

        public static Connector Instance
        {
            get
            {
                if (instance == null)
                    instance = new Connector();
                return instance;
            }
        }

        /// <summary>
        /// Checks wheter database connection can be established, and wheter app is up to date. If not, throw.
        /// </summary>
        public void ValidateVersion()
        {
            try
            {
                connectionEconomy.Open();
            }
            catch { MessageBox.Show("Nepodařilo se připojit k databázi aplikace. Zkontrolujte své připojení k internetu, firewall, a případně kontaktujte GMT."); throw; }

            string currentVersion = "";
            try
            {
                var query = new MySqlCommand(string.Format("SELECT `version` FROM `version`;"), connectionEconomy);
                using (var r = query.ExecuteReader())
                {
                    if (r.Read())
                    {
                        currentVersion = r[0].ToString();
                        if (currentVersion != conf.version)
                            throw new Exception();
                    }
                }
                connectionEconomy.Close();
            }
            catch { MessageBox.Show(string.Format("Zdá se, že vaše aplikace je zastaralá. Aktuální verze je {0}, stáhněte si ji.", currentVersion)); throw; }
        }
    }
}
