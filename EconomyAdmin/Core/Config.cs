using System;
using System.Text;
using System.IO;
using System.Windows;
using System.Xml;
using System.Security;

namespace EconomyAdmin.Core
{
    class Config
    {
        private static Config instance;

        private Config()
        {
            LoadSettings();
        }

        public static Config Instance
        {
            get
            {
                if (instance == null)
                    instance = new Config();
                return instance;
            }
        }

        private XmlDocument xml = new XmlDocument();

        public string hostEconomy = "127.0.0.1";
        public string databaseEconomy = "economy";
        public int portEconomy = 3306;

        public string hostAuth = "127.0.0.1";
        public string databaseAuth = "auth";
        public int portAuth = 3306;

        public SecureString userLogin = Utilities.ToSecureString("");
        public SecureString userPassword = Utilities.ToSecureString("");
        public bool saveCredentials = false;
        public int companyID = -1;

        public SecureString sqlLoginAuth = Utilities.ToSecureString("economy_test");
        public SecureString sqlPasswordAuth = Utilities.ToSecureString("economy");
        public SecureString sqlLoginEconomy = Utilities.ToSecureString("economy_test");
        public SecureString sqlPasswordEconomy = Utilities.ToSecureString("economy");
        public string version = "1.0.0";

        /// <summary>
        /// Loads saved user settings from UserSettings.xml. If file is not found default values are used.
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                using (var sr = new StreamReader("Config/Config.xml"))
                {
                    string xmlString = sr.ReadToEnd();
                    sr.Close();
                    xml.LoadXml(xmlString);
                }

                hostEconomy = xml.GetElementsByTagName("hostEconomy")[0].InnerText;
                databaseEconomy = xml.GetElementsByTagName("databaseEconomy")[0].InnerText;
                portEconomy = int.Parse(xml.GetElementsByTagName("portEconomy")[0].InnerText);

                hostAuth = xml.GetElementsByTagName("hostAuth")[0].InnerText;
                databaseAuth = xml.GetElementsByTagName("databaseAuth")[0].InnerText;
                portAuth = int.Parse(xml.GetElementsByTagName("portAuth")[0].InnerText);

                userLogin = Utilities.DecryptString(xml.GetElementsByTagName("userLogin")[0].InnerText);
                userPassword = Utilities.DecryptString(xml.GetElementsByTagName("userPassword")[0].InnerText);
                saveCredentials = xml.GetElementsByTagName("saveCredentials")[0].InnerText.ToString().ToLower() == "true";
                companyID = int.Parse(xml.GetElementsByTagName("companyID")[0].InnerText);
            }
            catch (Exception e)
            {
                MessageBox.Show("Chyba při pokusu o otevření a načtení konfiguračního souboru. Zkuste znovu stáhnout aplikaci.\n\n" + e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Saves nearly everything user has entered into form as default configuration for next startup of application.
        /// </summary>
        public void SaveUserSettings()
        {
            if (!Directory.Exists("Config"))
                Directory.CreateDirectory("Config");
            using (TextWriter tw = new StreamWriter("Config/Config.xml", false, Encoding.UTF8))
            {
                xml.GetElementsByTagName("hostEconomy")[0].InnerText = hostEconomy;
                xml.GetElementsByTagName("database")[0].InnerText = databaseEconomy;
                xml.GetElementsByTagName("portEconomy")[0].InnerText = portEconomy.ToString();

                xml.GetElementsByTagName("hostAuth")[0].InnerText = hostAuth;
                xml.GetElementsByTagName("databaseAuth")[0].InnerText = databaseAuth;
                xml.GetElementsByTagName("portAuth")[0].InnerText = portAuth.ToString();

                xml.GetElementsByTagName("userLogin")[0].InnerText = userLogin.ToString();
                xml.GetElementsByTagName("userPassword")[0].InnerText = userPassword.ToString();
                xml.GetElementsByTagName("saveCredentials")[0].InnerText = saveCredentials.ToString();
                xml.GetElementsByTagName("companyID")[0].InnerText = companyID.ToString();

                xml.Save(tw);
                tw.Close();
            }
        }
    }
}
