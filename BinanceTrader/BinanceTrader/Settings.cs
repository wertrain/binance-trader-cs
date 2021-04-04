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
        /// 個人 API の URL
        /// </summary>
        public string PrivateApiUrl { get; set; }

        /// <summary>
        /// Binance API のキー
        /// </summary>
        public string BinanceApiKey { get; set; }

        /// <summary>
        /// Binance API のシークレットキー
        /// </summary>
        public string BinanceSecretKey { get; set; }

        /// <summary>
        /// 仮想購入
        /// </summary>
        public class VirtualPurchaseInfo
        {
            /// <summary>
            /// 銘柄名
            /// </summary>
            public string Symbol { get; set; }

            /// <summary>
            /// コイン名
            /// </summary>
            public string BaseAsset { get; set; }

            /// <summary>
            /// 値付けコイン名
            /// </summary>
            public string QuoteAsset { get; set; }

            /// <summary>
            /// 購入価格
            /// </summary>
            public float PurchasePrice { get; set; }

            /// <summary>
            /// 購入日
            /// </summary>
            public DateTime PurchaseDate { get; set; }
        }

        /// <summary>
        /// 仮想購入情報
        /// </summary>
        public List<VirtualPurchaseInfo> VirtualPurchases;

        /// <summary>
        /// 適用されているテーマ名
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        /// 自動更新するかどうか
        /// </summary>
        public bool IsAutoUpdate { get; set; }

        /// <summary>
        /// 自動更新間隔（秒）
        /// </summary>
        public double AutoUpdateIntervalSeconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private Settings()
        {
            VirtualPurchases = new List<VirtualPurchaseInfo>();
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
                VirtualPurchases = new List<VirtualPurchaseInfo>(),
                ThemeName = "Generic",
                IsAutoUpdate = true,
                AutoUpdateIntervalSeconds = 60
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
                    VirtualPurchases = settings.VirtualPurchases;
                    ThemeName = settings.ThemeName;
                    IsAutoUpdate = settings.IsAutoUpdate;
                    AutoUpdateIntervalSeconds = settings.AutoUpdateIntervalSeconds;
                    return this;
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
