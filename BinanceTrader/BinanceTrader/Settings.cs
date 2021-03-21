using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BinanceTrader
{
    public class Settings
    {
        /// <summary>
        /// 
        /// </summary>
        public string PrivateApiUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BinanceApiKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BinanceSecretKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private Settings()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Settings Default()
        {
            return new Settings()
            {
                PrivateApiUrl = string.Empty,
                BinanceApiKey = string.Empty,
                BinanceSecretKey = string.Empty,
            };
        }

        /// <summary>
        /// インスタンス
        /// </summary>
        public static Settings Instance { get; } = new Settings();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Settings Load()
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));

            try
            {
                var assembly = Assembly.GetEntryAssembly();
                var filePath = Path.GetDirectoryName(assembly.Location) + Path.DirectorySeparatorChar + "settings.xml";

                using (var sr = new StreamReader(filePath, new UTF8Encoding(false)))
                {
                    var settings = (Settings)serializer.Deserialize(sr);
                    PrivateApiUrl = settings.PrivateApiUrl;
                    BinanceApiKey = settings.BinanceApiKey;
                    BinanceSecretKey = settings.BinanceSecretKey;
                }
            }
            catch (Exception)
            {

            }
            return Default();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public bool Save()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));

            try
            {
                var assembly = Assembly.GetEntryAssembly();
                var filePath = Path.GetDirectoryName(assembly.Location) + Path.DirectorySeparatorChar + "settings.xml";

                using (var sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
                {
                    serializer.Serialize(sw, this);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
