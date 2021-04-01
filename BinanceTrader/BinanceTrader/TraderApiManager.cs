using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    public class TraderApiResponse<T> where T : class
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
            var jsonObject = JsonSerializer.Deserialize<T>(Raw, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            return jsonObject;
        }

        /// <summary>
        /// オブジェクトのリストに変換
        /// </summary>
        /// <returns></returns>
        public virtual List<T> ToObjectList()
        {
            var jsonObjects = new List<T>();

            var jsonStrings = Raw.Split('\n');
            foreach (var jsonString in jsonStrings)
            {
                var jsonPrices = JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                jsonObjects.Add(jsonPrices);
            }

            return jsonObjects;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rawResponse"></param>
        public TraderApiResponse(string rawResponse)
        {
            Raw = rawResponse;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TraderApiListResponse<T> where T : class
    {
        /// <summary>
        /// レスポンスの生データ
        /// </summary>
        public string Raw { get; }

        /// <summary>
        /// オブジェクトのリストに変換
        /// </summary>
        /// <returns></returns>
        public virtual List<T> ToObject()
        {
            var jsonObjects = new List<T>();

            var jsonStrings = Raw.Split('\n');
            foreach (var jsonString in jsonStrings)
            {
                var jsonPrices = JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                jsonObjects.Add(jsonPrices);
            }

            return jsonObjects;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rawResponse"></param>
        public TraderApiListResponse(string rawResponse)
        {
            Raw = rawResponse;
        }
    }

    /// <summary>
    /// キャッシュ
    /// </summary>
    public class TraderApiCache
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
        public TraderApiCache(Client binanceApiClient, PrivateApiClient privateApiClient)
        {
            Binance = binanceApiClient;
            Private = privateApiClient;
            Cached = new Dictionary<string, CachedData>();
        }

        /// <summary>
        /// GetAllTickers の呼び出し
        /// </summary>
        /// <returns></returns>
        public TraderApiResponse<List<PricePair>> GetAllTickers()
        {
            string cacheKey = "GetAllTickers";

            if (Cached.ContainsKey(cacheKey))
            {
                var cache = Cached[cacheKey];
                var cachedDate = cache.Date;
                var span = DateTime.Now - cachedDate;

                if (span.Seconds < 60 * 60)
                {
                    return new TraderApiResponse<List<PricePair>>(cache.Data);
                }
            }

            var data = Binance.GetAllTickers();

            Cached[cacheKey] = new CachedData(DateTime.Now, data);

            return new TraderApiResponse<List<PricePair>>(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TraderApiResponse<System.Dynamic.ExpandoObject> GetExchangeInfo()
        {
            string cacheKey = "GetExchangeInfo";

            if (Cached.ContainsKey(cacheKey))
            {
                var cache = Cached[cacheKey];
                var cachedDate = cache.Date;
                var span = DateTime.Now - cachedDate;

                if (span.Seconds < 60 * 60)
                {
                    return new TraderApiResponse<System.Dynamic.ExpandoObject>(cache.Data);
                }
            }

            var data = Binance.GetExchangeInfo();

            Cached[cacheKey] = new CachedData(DateTime.Now, data);

            return new TraderApiResponse<System.Dynamic.ExpandoObject>(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TraderApiListResponse<List<PricePair>> GetPastTimeTickers()
        {
            string cacheKey = "GetPastTimeTickers";

            if (Cached.ContainsKey(cacheKey))
            {
                var cache = Cached[cacheKey];
                var cachedDate = cache.Date;
                var span = DateTime.Now - cachedDate;

                if (span.Seconds < 60 * 60)
                {
                    return new TraderApiListResponse<List<PricePair>>(cache.Data);
                }
            }

            var data = Private.GetPastTimeTickers();

            Cached[cacheKey] = new CachedData(DateTime.Now, data);

            return new TraderApiListResponse<List<PricePair>>(data);
        }

        /// <summary>
        /// キャッシュデータ
        /// </summary>
        private Dictionary<string, CachedData> Cached;

        /// <summary>
        /// クライアント
        /// </summary>
        private Client Binance { get; set; }

        /// <summary>
        /// クライアント
        /// </summary>
        private PrivateApiClient Private { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PrivateApiClient
    {
        /// <summary>
        /// ダミーデータを使用するかどうか
        /// </summary>
        public bool IsUseDummyData { get; set; }

        /// <summary>
        /// サーバーから過去の価格情報をダウンロード
        /// </summary>
        /// <returns></returns>
        public string GetPastTimeTickers()
        {
            if (IsUseDummyData)
            {
                return readDummyPrices();
            }

            var api = Settings.Instance.PrivateApiUrl;
            if (string.IsNullOrEmpty(api)) return string.Empty;

            var request = System.Net.WebRequest.Create(api) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json;";

            var httpResponse = request.GetResponse() as System.Net.HttpWebResponse;

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd().Trim();
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 開発用ダミーデータをファイルから読み込み
        /// </summary>
        /// <returns></returns>
        private string readDummyPrices()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            using (StreamReader sr = new StreamReader(System.IO.Path.GetDirectoryName(assembly.Location) + System.IO.Path.DirectorySeparatorChar + "dummy_prices.txt", Encoding.GetEncoding("utf-8")))
            {
                return sr.ReadToEnd();
            }
        }
    }

    /// <summary>
    /// Binance API 管理
    /// </summary>
    class TraderApiManager
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static TraderApiManager Instance { get; } = new TraderApiManager();

        /// <summary>
        /// API クライアント
        /// </summary>
        public Client Binance { get; private set; }

        /// <summary>
        /// Private API クライアント
        /// </summary>
        public PrivateApiClient Private { get; set; }

        /// <summary>
        /// API キャッシュ
        /// </summary>
        public TraderApiCache Cache { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private TraderApiManager()
        {
            Binance = new Client();
            Private = new PrivateApiClient();
            Cache = new TraderApiCache(Binance, Private);
        }
    }
}
