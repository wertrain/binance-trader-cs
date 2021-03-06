using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Binance.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class BinanceApiException : Exception
    {
        /// <summary>
        /// ステータスコード
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="statusCode"></param>
        public BinanceApiException(HttpStatusCode statusCode) => StatusCode = statusCode;
    }

    /// <summary>
    /// Binance API のラッパー
    /// </summary>
    public class Client
    {
        /// <summary>
        /// エンドポイント
        /// </summary>
        private readonly List<string> EndPoints = new List<string>()
        {
            "https://api.binance.com",
            "https://api1.binance.com",
            "https://api2.binance.com",
            "https://api3.binance.com",
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Client()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        public Client(string apiKey, string secretKey)
        {
            ApiKey = apiKey;
            SecretKey = secretKey;
        }

        /// <summary>
        /// ダミーデータを使用するかどうか
        /// </summary>
        public bool IsUseDummyData { get; set; }

        /// <summary>
        /// Binance API のキー
        /// </summary>
        private string ApiKey { get; set; }

        /// <summary>
        /// Binance API のシークレットキー
        /// </summary>
        private string SecretKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetEntPoint()
        {
            return EndPoints[0];
        }

        /// <summary>
        /// 全てのコインの価格を取得
        /// </summary>
        /// <returns></returns>
        public string GetAllTickers()
        {
            if (IsUseDummyData)
            {
                return readDummyPrice();
            }

            var api = "/api/v3/ticker/price";

            var request = WebRequest.Create(GetEntPoint() + api) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json;";

            var httpResponse = request.GetResponse() as HttpWebResponse;

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            else
            {
                throw new BinanceApiException(httpResponse.StatusCode);
            }
        }

        /// <summary>
        /// 通貨情報を取得
        /// </summary>
        /// <returns></returns>
        public string GetExchangeInfo()
        {
            if (IsUseDummyData)
            {
                return readDummyExchangeInfo();
            }

            var api = "/api/v3/exchangeInfo";

            var request = WebRequest.Create(GetEntPoint() + api) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json;";

            var httpResponse = request.GetResponse() as HttpWebResponse;

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            else
            {
                throw new BinanceApiException(httpResponse.StatusCode);
            }
        }

        /// <summary>
        /// 開発用ダミーデータをファイルから読み込み
        /// </summary>
        /// <returns></returns>
        private string readDummyPrice()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            using (StreamReader sr = new StreamReader(Path.GetDirectoryName(assembly.Location) + Path.DirectorySeparatorChar + "dummy_price.txt", System.Text.Encoding.GetEncoding("utf-8")))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 開発用のダミーデータをファイルから読み込み
        /// </summary>
        /// <returns></returns>
        private string readDummyExchangeInfo()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            using (StreamReader sr = new StreamReader(Path.GetDirectoryName(assembly.Location) + Path.DirectorySeparatorChar + "dummy_exchangeinfo.txt", System.Text.Encoding.GetEncoding("utf-8")))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
