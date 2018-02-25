using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Security;
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

        private int accountID = -1;
        private int companyID = -1;

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

        public int CompanyID
        {
            get => companyID;
            set
            {
                ReloadCompany();
                companyID = value;
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
            catch
            {
                MessageBox.Show("Nepodařilo se připojit k databázi aplikace. Zkontroluj své připojení k internetu, firewall, a případně kontaktuj GMT.");
                throw;
            }

            string currentVersion = "";
            try
            {
                var query = new MySqlCommand("SELECT `version` FROM `version`;", connectionEconomy);
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
            catch
            {
                MessageBox.Show(string.Format("Zdá se, že aplikace je zastaralá. Aktuální verze je {0}, stáhni si ji.", currentVersion));
                throw;
            }
        }

        /// <summary>
        /// Checks wheter provided login credentials are correct and wheter given user is not banned. If user is valid, stores his account ID.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns>Error code. 0=OK, 1=InvalidCredentials, 2=AccountBanned, 3=IPBanned, 4=CheckFailed</returns>
        public int IsUserValid(SecureString login, SecureString password)
        {
            //try
            {
                accountID = -1;
                CompanyID = -1;
                /// NIY:
                // Can connect?
                // Account+password exists?
                // Is account not banned?
                // Is IP not banned?
                connectionAuth.Open();

                var query = new MySqlCommand(string.Format(@"SELECT id FROM account WHERE sha_pass_hash=SHA1(CONCAT('{0}', ':', '{1}'));",
                    Utilities.ToInsecureString(login), Utilities.ToInsecureString(password)), connectionAuth);
                using (var r = query.ExecuteReader())
                {
                    if (r.Read())
                        accountID = Convert.ToInt32(r[0]);
                    else
                    {
                        connectionAuth.Close();
                        return 1;
                    }
                }

                connectionAuth.Close();

                return 0;
            }
            //catch { return 4; }
        }

        public Dictionary<string, int> GetAvailableCompanies()
        {
            var result = new Dictionary<string, int>();
            try
            {
                connectionEconomy.Open();
                var query = new MySqlCommand(string.Format(@"SELECT company.name, company_id
FROM user_company_access
JOIN `user` ON user.accountid = user_company_access.user_accountid
JOIN company ON company.id = user_company_access.company_id
WHERE `user`.isvalid = 1
AND company.isvalid = 1
AND `user`.accountid = {0};", accountID), connectionEconomy);
                using (var r = query.ExecuteReader())
                {
                    while (r.Read())
                    {
                        result.Add(r[0].ToString(), Convert.ToInt32(r[1]));
                    }
                }
                connectionEconomy.Close();
            }
            catch
            {
                MessageBox.Show("Nepodařilo se získat seznam společností, ke kterým máš udělen přístup. Aplikace bude ukončena.");
                throw;
            }
            return result;
        }

        public void ReloadCompany()
        {
            if (companyID > 0)
            {

            }
        }
    }
}
