using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using System.IO;
using System.Collections.ObjectModel;

namespace BinanceTrader.Controls
{
    /// <summary>
    /// デバッグ用の定数
    /// </summary>
    class LocalConstants
    {
        public static readonly bool UseDummyData = true;
    }

    /// <summary>
    /// TickerList.xaml の相互作用ロジック
    /// </summary>
    public partial class TickerList : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        private class JsonPricePair
        {
            [JsonPropertyName("symbol")]
            public string Symbol { get; set; }
            [JsonPropertyName("price")]
            public string Price { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class TickerPrices
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
            /// 時間ごとの価格
            /// </summary>
            public List<float> Prices { get; set; }

            /// <summary>
            /// 時間ごとの変動率
            /// </summary>
            public List<float> Rates { get; set; }

            /// <summary>
            /// 描画色
            /// </summary>
            public List<Brush> PriceColors { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public TickerPrices()
            {
                Prices = new List<float>();
                Rates = new List<float>();
                PriceColors = new List<Brush>();
            }
        }

        /// <summary>
        /// シンボル情報
        /// </summary>
        private class SymbolInfo
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
            /// 状態
            /// </summary>
            public string Status { get; set; }
        }

        /// <summary>
        /// リストに紐づく価格情報
        /// </summary>
        private ObservableCollection<TickerPrices> Prices;

        /// <summary>
        /// シンボル情報
        /// </summary>
        private Dictionary<string, SymbolInfo> Symbols;

        /// <summary>
        /// 
        /// </summary>
        public TickerList()
        {
            InitializeComponent();

            Prices = new ObservableCollection<TickerPrices>();
            Symbols = new Dictionary<string, SymbolInfo>();

            UpdateSymbols();
            UpdateTickers();

            VirtualPurchaseCommand = new ContextMenuCommand<TickerPrices>(selectVirtualPurchase);

            _listViewTickers.DataContext = this;
            _listViewTickers.ItemsSource = Prices;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateSymbols()
        {
            var exchangeInfo = LocalConstants.UseDummyData ? 
                readDummyExchangeInfo() : 
                BinanceApiManager.Instance.Client.GetExchangeInfo();
            if (string.IsNullOrEmpty(exchangeInfo)) return;
            dynamic jsonExchangeInfo = JsonSerializer.Deserialize<System.Dynamic.ExpandoObject>(exchangeInfo);

            var symbols = ((IDictionary<string, object>)jsonExchangeInfo)["symbols"];
            foreach (var value in ((JsonElement)symbols).EnumerateArray())
            {
                if (value.ValueKind == JsonValueKind.Object)
                {
                    var symbolInfo = new SymbolInfo();
                    foreach (var prop in value.EnumerateObject())
                    {
                        switch (prop.Name)
                        {
                            case "symbol": symbolInfo.Symbol = prop.Value.ToString(); break;
                            case "baseAsset": symbolInfo.BaseAsset = prop.Value.ToString(); break;
                            case "quoteAsset": symbolInfo.QuoteAsset = prop.Value.ToString(); break;
                            case "status": symbolInfo.Status = prop.Value.ToString(); break;
                        }
                    }
                    Symbols.Add(symbolInfo.Symbol, symbolInfo);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateTickers()
        {
            var jsonPricesList = new List<List<JsonPricePair>>();

            // 現在の価格を Binance API を使用して取得
            {
                var jsonString = LocalConstants.UseDummyData ?
                    readDummyPrice() :
                    BinanceApiManager.Instance.Cache.GetAllTickers().Raw;
                if (string.IsNullOrEmpty(jsonString)) return;
                var jsonPrices = JsonSerializer.Deserialize<List<JsonPricePair>>(jsonString, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                jsonPricesList.Add(jsonPrices);
            }

            // 過去の価格をサーバーから取得
            var jsonPrice = LocalConstants.UseDummyData ?
                readDummyPrices():
                downloadJsonPrices();
            if (string.IsNullOrEmpty(jsonPrice)) return;
            var jsonStrings = jsonPrice.Split('\n');
            foreach (var jsonString in jsonStrings)
            {
                var jsonPrices = JsonSerializer.Deserialize<List<JsonPricePair>>(jsonString, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                jsonPricesList.Add(jsonPrices);
            }

            updatePrices(jsonPricesList);
        }

        /// <summary>
        /// サーバーから過去の価格情報をダウンロード
        /// </summary>
        /// <returns></returns>
        private string downloadJsonPrices()
        {
            var api = Settings.Instance.PrivateApiUrl;
            if (string.IsNullOrEmpty(api)) return string.Empty;

            var request = WebRequest.Create(api) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json;";

            var httpResponse = request.GetResponse() as HttpWebResponse;

            if (httpResponse.StatusCode == HttpStatusCode.OK)
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
        private string readDummyPrice()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return string.Empty;

            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            using (StreamReader sr = new StreamReader(System.IO.Path.GetDirectoryName(assembly.Location) + System.IO.Path.DirectorySeparatorChar + "dummy_price.txt", Encoding.GetEncoding("utf-8")))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 開発用ダミーデータをファイルから読み込み
        /// </summary>
        /// <returns></returns>
        private string readDummyPrices()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return string.Empty;

            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            using (StreamReader sr = new StreamReader(System.IO.Path.GetDirectoryName(assembly.Location) + System.IO.Path.DirectorySeparatorChar + "dummy_prices.txt", Encoding.GetEncoding("utf-8")))
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
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return string.Empty;

            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            using (StreamReader sr = new StreamReader(System.IO.Path.GetDirectoryName(assembly.Location) + System.IO.Path.DirectorySeparatorChar + "dummy_exchangeinfo.txt", Encoding.GetEncoding("utf-8")))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 価格情報を更新
        /// </summary>
        /// <param name="jsonPricePairsList"></param>
        private void updatePrices(List<List<JsonPricePair>> jsonPricePairsList)
        {
            var priceMap = new Dictionary<string, List<float>>();
            foreach (var jsonPricePairs in jsonPricePairsList)
            {
                foreach (var jsonPricePair in jsonPricePairs)
                {
                    var price = float.Parse(jsonPricePair.Price);
                    
                    if (!priceMap.ContainsKey(jsonPricePair.Symbol))
                    {
                        var prices = new List<float>();
                        priceMap.Add(jsonPricePair.Symbol, prices);
                    }
                    priceMap[jsonPricePair.Symbol].Add(price);
                }
            }

            Prices.Clear();
            foreach (var pair in priceMap)
            {
                var rates = new List<float>();
                var priceColors = new List<Brush>();

                // 末尾（現在は24時間前の情報）との比較でパーセンテージを表示
                var target = pair.Value.LastOrDefault();
                for (int i = 0; i < pair.Value.Count - 1; ++i)
                {
                    var current = pair.Value[i];
                    //var target = pair.Value[i + 1];
                    var percent = ((current - target) / target);
                    rates.Add(percent);

                    if (percent == 0)
                    {
                        priceColors.Add(Brushes.DarkSlateGray);
                    }
                    else if (percent > 0.0)
                    {
                        priceColors.Add(Brushes.BlueViolet);
                    }
                    else
                    {
                        priceColors.Add(Brushes.OrangeRed);
                    }
                }
                // なので末尾は比較対象がない
                {
                    priceColors.Add(Brushes.DarkSlateGray);
                    rates.Add(0);
                }

                var baseAsset = string.Empty;
                var quoteAsset = string.Empty;
                if (Symbols.ContainsKey(pair.Key))
                {
                    baseAsset = Symbols[pair.Key].BaseAsset;
                    quoteAsset = Symbols[pair.Key].QuoteAsset;
                }

                Prices.Add(new TickerPrices()
                {
                    Symbol = pair.Key,
                    BaseAsset = baseAsset,
                    QuoteAsset = quoteAsset,
                    Prices = pair.Value,
                    Rates = rates,
                    PriceColors = priceColors
                });
            }
        }

        /// <summary>
        /// 仮想購入が選択された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectVirtualPurchase(TickerPrices item)
        {
            OnSelectVirtualPurchase?.Invoke(this, new VirtualPurchaseEventArgs()
            {
                Symbol = item.Symbol,
                BaseAsset = item.BaseAsset,
                QuoteAsset = item.QuoteAsset,
                Price = item.Prices.FirstOrDefault()
            });
        }

        /// <summary>
        /// リストの選択が変更された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewTickers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tickerPrices = e.AddedItems[0] as TickerPrices;

            SelectionChanged?.Invoke(this, new TickerSelectionChangedEventArgs()
            {
                Symbol = tickerPrices.Symbol,
                Prices = tickerPrices.Prices,
                Rates = tickerPrices.Rates
            });
        }

        #region イベント関連

        /// <summary>
        /// 銘柄の選択変更イベント引数
        /// </summary>
        public class TickerSelectionChangedEventArgs
        {
            /// <summary>
            /// 銘柄名
            /// </summary>
            public string Symbol { get; set; }

            /// <summary>
            /// 時間ごとの価格
            /// </summary>
            public List<float> Prices { get; set; }

            /// <summary>
            /// 時間ごとの変動率
            /// </summary>
            public List<float> Rates { get; set; }
        }

        /// <summary>
        /// リストから銘柄を選択したときのイベント
        /// </summary>
        public EventHandler<TickerSelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// 仮想購入が選択されたときのイベント引数
        /// </summary>
        public class VirtualPurchaseEventArgs
        {
            /// <summary>
            /// 銘柄名
            /// </summary>
            public string Symbol { get; set; }

            /// <summary>
            /// 現在価格
            /// </summary>
            public float Price { get; set; }

            /// <summary>
            /// コイン名
            /// </summary>
            public string BaseAsset { get; set; }

            /// <summary>
            /// 値付けコイン名
            /// </summary>
            public string QuoteAsset { get; set; }
        }

        /// <summary>
        /// 仮想購入が選択されたときのイベント
        /// </summary>
        public EventHandler<VirtualPurchaseEventArgs> OnSelectVirtualPurchase;

        #endregion

        #region コマンド関連

        /// <summary>
        /// リストアイテム選択時のコマンド
        /// </summary>
        public ContextMenuCommand<TickerPrices> VirtualPurchaseCommand { get; set; }

        /// <summary>
        /// コマンド
        /// </summary>
        public class ContextMenuCommand<T> : ICommand
        {
            /// <summary>
            /// 実行可能状態変更イベント
            /// </summary>
            #pragma warning disable 67
            public event EventHandler CanExecuteChanged;
            #pragma warning restore 67

            /// <summary>
            /// アクション
            /// </summary>
            private Action<T> action_;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="action">コマンド実行時アクション</param>
            public ContextMenuCommand(Action<T> action)
            {
                action_ = action;
            }

            /// <summary>
            /// 実行可能かを取得します
            /// </summary>
            /// <param name="parameter">パラメータ</param>
            /// <returns>実行可能な場合、trueを返します</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <summary>
            /// コマンドを実行します
            /// </summary>
            /// <param name="parameter">パラメータ</param>
            public void Execute(object parameter)
            {
                if (action_ != null)
                {
                    action_.Invoke((T)parameter);
                }
            }
        }

        #endregion
    }
}
