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

        public string host = "127.0.0.1";
        public string database = "economy";
        public int port = 3306;

        public SecureString userLogin = Utilities.ToSecureString("");
        public SecureString userPassword = Utilities.ToSecureString("");
        public int companyID = -1;

        public SecureString sqlLogin = Utilities.ToSecureString("economy_test");
        public SecureString sqlPassword = Utilities.ToSecureString("economy");
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

                host = xml.GetElementsByTagName("host")[0].InnerText;
                database = xml.GetElementsByTagName("database")[0].InnerText;
                port = int.Parse(xml.GetElementsByTagName("port")[0].InnerText);

                userLogin = Utilities.DecryptString(xml.GetElementsByTagName("userLogin")[0].InnerText);
                userPassword = Utilities.DecryptString(xml.GetElementsByTagName("userPassword")[0].InnerText);
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
                xml.GetElementsByTagName("host")[0].InnerText = host;
                xml.GetElementsByTagName("database")[0].InnerText = database;
                xml.GetElementsByTagName("port")[0].InnerText = port.ToString();

                xml.GetElementsByTagName("userLogin")[0].InnerText = userLogin.ToString();
                xml.GetElementsByTagName("userPassword")[0].InnerText = userPassword.ToString();
                xml.GetElementsByTagName("companyID")[0].InnerText = companyID.ToString();

                xml.Save(tw);
                tw.Close();
            }
        }
    }
}
