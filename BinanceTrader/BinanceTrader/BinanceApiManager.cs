using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Binance.Api;

namespace BinanceTrader
{
    /// <summary>
    /// 
    /// </summary>
    public class PricePair
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
        [JsonPropertyName("price")]
        public string Price { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BinanceApiResponse<T> where T : class
    {
        /// <summary>
        /// レスポンスの生データ
        /// </summary>
        public string Raw { get; }
        
        /// <summary>
        /// オブジェクトに変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T ToObject()
        {
            if (typeof(T) == typeof(List<PricePair>))
            {
                var jsonPrices = JsonSerializer.Deserialize<T>(Raw, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                return jsonPrices;
            }
            return null;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rawResponse"></param>
        public BinanceApiResponse(string rawResponse)
        {
            Raw = rawResponse;
        }
    }

    /// <summary>
    /// キャッシュ
    /// </summary>
    public class BinanceApiCache
    {
        /// <summary>
        /// キャッシュデータ
        /// </summary>
        class CachedData
        {
            /// <summary>
            /// キャッシュされた日時
            /// </summary>
            public DateTime Date { get; }

            /// <summary>
            /// キャッシュされたデータ
            /// </summary>
            public string Data { get; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="date"></param>
            /// <param name="data"></param>
            public CachedData(DateTime date, string data)
            {
                Date = date;
                Data = data;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="client"></param>
        public BinanceApiCache(Binance.Api.Client client)
        {
            Client = client;
            Cached = new Dictionary<string, CachedData>();
        }

        /// <summary>
        /// GetAllTickers の呼び出し
        /// </summary>
        /// <returns></returns>
        public BinanceApiResponse<List<PricePair>> GetAllTickers()
        {
            string cacheKey = "GetAllTickers";

            if (Cached.ContainsKey(cacheKey))
            {
                var cache = Cached[cacheKey];
                var cachedDate = cache.Date;
                var span = DateTime.Now - cachedDate;

                if (span.Seconds < 60 * 60)
                {
                    return new BinanceApiResponse<List<PricePair>>(cache.Data);
                }
            }

            var data = Client.GetAllTickers();

            Cached[cacheKey] = new CachedData(DateTime.Now, data);

            return new BinanceApiResponse<List<PricePair>>(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BinanceApiResponse<System.Dynamic.ExpandoObject> GetExchangeInfo()
        {
            string cacheKey = "GetExchangeInfo";

            if (Cached.ContainsKey(cacheKey))
            {
                var cache = Cached[cacheKey];
                var cachedDate = cache.Date;
                var span = DateTime.Now - cachedDate;

                if (span.Seconds < 60 * 60)
                {
                    return new BinanceApiResponse<System.Dynamic.ExpandoObject>(cache.Data);
                }
            }

            var data = Client.GetExchangeInfo();

            Cached[cacheKey] = new CachedData(DateTime.Now, data);

            return new BinanceApiResponse<System.Dynamic.ExpandoObject>(data);
        }

        /// <summary>
        /// キャッシュデータ
        /// </summary>
        private Dictionary<string, CachedData> Cached;

        /// <summary>
        /// クライアント
        /// </summary>
        private Binance.Api.Client Client { get; set; }
    }

    /// <summary>
    /// Binance API 管理
    /// </summary>
    class BinanceApiManager
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static BinanceApiManager Instance { get; } = new BinanceApiManager();

        /// <summary>
        /// API クライアント
        /// </summary>
        public Binance.Api.Client Client { get; private set; }

        /// <summary>
        /// API キャッシュ
        /// </summary>
        public BinanceApiCache Cache { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private BinanceApiManager()
        {
            Client = new Binance.Api.Client();
            Cache = new BinanceApiCache(Client);
        }
    }
}
