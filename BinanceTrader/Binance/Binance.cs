using System;

namespace Binance.Api
{
    /// <summary>
    /// Binance API のラッパー
    /// </summary>
    public class Binance
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        public Binance(string apiKey, string secretKey)
        {
            ApiKey = apiKey;
            SecretKey = secretKey;
        }

        /// <summary>
        /// Binance API のキー
        /// </summary>
        private string ApiKey { get; set; }

        /// <summary>
        /// Binance API のシークレットキー
        /// </summary>
        private string SecretKey { get; set; }
    }
}
