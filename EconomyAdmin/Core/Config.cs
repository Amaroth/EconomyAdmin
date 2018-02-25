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

        public string hostEconomy;
        public string databaseEconomy;
        public int portEconomy;

        public string hostAuth;
        public string databaseAuth;
        public int portAuth;

        public SecureString userLogin;
        public SecureString userPassword;
        public bool saveCredentials;

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
            }
            catch
            {
                MessageBox.Show("Chyba při pokusu o otevření a načtení konfiguračního souboru. Zkus znovu stáhnout aplikaci.");
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
                xml.GetElementsByTagName("saveCredentials")[0].InnerText = saveCredentials.ToString();
                if (saveCredentials)
                    xml.GetElementsByTagName("userLogin")[0].InnerText = Utilities.EncryptString(userLogin);
                else
                    xml.GetElementsByTagName("userLogin")[0].InnerText = "";
                if (saveCredentials)
                    xml.GetElementsByTagName("userPassword")[0].InnerText = Utilities.EncryptString(userPassword);
                else
                    xml.GetElementsByTagName("userPassword")[0].InnerText = "";

                xml.Save(tw);
                tw.Close();
            }
        }
    }
}
