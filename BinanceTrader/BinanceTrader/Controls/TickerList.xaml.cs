using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Text.Json;
using System.Collections.ObjectModel;
using BinanceTrader.Controls.Common;

namespace BinanceTrader.Controls
{
    /// <summary>
    /// TickerList.xaml の相互作用ロジック
    /// </summary>
    public partial class TickerList : ControlBase
    {
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
            /// コンストラクタ
            /// </summary>
            public TickerPrices()
            {
                Prices = new List<float>();
                Rates = new List<float>();
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
        /// シンボル情報
        /// </summary>
        private Dictionary<string, SymbolInfo> Symbols;

        /// <summary>
        /// リストに紐づく価格情報
        /// </summary>
        private Dictionary<string, TickerPrices> Prices;

        /// <summary>
        /// 価格情報を保持するコンテナ
        /// </summary>
        private ObservableCollection<TickerPrices> PricesContainer;

        /// <summary>
        /// 
        /// </summary>
        public TickerList()
        {
            InitializeComponent();

            Symbols = new Dictionary<string, SymbolInfo>();
            Prices = new Dictionary<string, TickerPrices>();
            PricesContainer = new ObservableCollection<TickerPrices>();

            UpdateSymbols();
            UpdateTickers();

            TraderDispatcherTimer.Instance.Tick += Timer_Tick;

            VirtualPurchaseCommand = new ContextMenuCommand<TickerPrices>(
                SelectVirtualPurchase,
                SelectVirtualPurchaseConverter);

            _listViewTickers.DataContext = this;
            _listViewTickers.ItemsSource = PricesContainer;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateSymbols()
        {
            var response = TraderApiManager.Instance.Cache.GetExchangeInfo();
            dynamic jsonExchangeInfo = response.ToObject();

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
            var jsonPricesList = new List<List<PricePair>>();

            // 現在の価格を Binance API を使用して取得
            {
                var response = TraderApiManager.Instance.Cache.GetAllTickers();
                jsonPricesList.Add(response.ToObject());
            }

            // 過去の価格をサーバーから取得
            try
            {
                var response = TraderApiManager.Instance.Cache.GetPastTimeTickers();
                jsonPricesList.AddRange(response.ToObject());
            }
            catch
            {
                Error("Failed to retrieve historical price data.");
            }

            UpdatePrices(jsonPricesList);
        }

        /// <summary>
        /// シンボルを選択
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool SelectSymbol(string symbol)
        {
            foreach (TickerPrices item in _listViewTickers.Items)
            {
                if (item.Symbol == symbol)
                {
                   var index =  _listViewTickers.Items.IndexOf(item);
                    _listViewTickers.SelectedIndex = index;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 価格情報を更新
        /// </summary>
        /// <param name="jsonPricePairsList"></param>
        private void UpdatePrices(List<List<PricePair>> jsonPricePairsList)
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

            foreach (var pair in priceMap)
            {
                var rates = new List<float>();

                // 末尾（現在は24時間前の情報）との比較でパーセンテージを表示
                var target = pair.Value.LastOrDefault();
                for (int i = 0; i < pair.Value.Count - 1; ++i)
                {
                    var current = pair.Value[i];
                    //var target = pair.Value[i + 1];
                    var percent = ((current - target) / target);
                    rates.Add(percent);
                }
                // なので末尾は比較対象がない
                {
                    rates.Add(0);
                }

                var baseAsset = string.Empty;
                var quoteAsset = string.Empty;
                if (Symbols.ContainsKey(pair.Key))
                {
                    baseAsset = Symbols[pair.Key].BaseAsset;
                    quoteAsset = Symbols[pair.Key].QuoteAsset;
                }

                Prices[pair.Key] = new TickerPrices()
                {
                    Symbol = pair.Key,
                    BaseAsset = baseAsset,
                    QuoteAsset = quoteAsset,
                    Prices = pair.Value,
                    Rates = rates
                };
            }

            #region リストの更新処理

            #if false
            #region 配列は維持したままでアイテムの中身を更新するパターン
            foreach (var item in Prices.Values)
            {
                var findItem = PricesContainer.Where(X => X.Symbol == item.Symbol).FirstOrDefault();

                if (findItem == null)
                {
                    PricesContainer.Add(item);
                }
                else
                {
                    findItem.Prices = item.Prices;
                    findItem.Rates = item.Rates;
                }
            }
            _listViewTickers.Items.Refresh();
            #endregion
            #else
            #region すべてクリアするパターン
            var selectedItem = _listViewTickers.SelectedItem as TickerPrices;

            PricesContainer.Clear();
            foreach (var item in Prices.Values)
            {
                PricesContainer.Add(item);
            }

            if (selectedItem != null)
            {
                _listViewTickers.SelectedItem = Prices[selectedItem.Symbol];
            }
            #endregion
            #endif

            #endregion
        }

        /// <summary>
        /// 仮想購入が選択された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectVirtualPurchase(TickerPrices item)
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
        /// 仮想購入が選択された時の値のコンバーター
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private TickerPrices SelectVirtualPurchaseConverter(object param)
        {
            var selectedItem = _listViewTickers.SelectedItem as TickerPrices;
            return Prices[selectedItem.Symbol];
        }

        /// <summary>
        /// リストの選択が変更された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewTickers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            var tickerPrices = e.AddedItems[0] as TickerPrices;

            SelectionChanged?.Invoke(this, new TickerSelectionChangedEventArgs()
            {
                Symbol = tickerPrices.Symbol,
                Prices = tickerPrices.Prices,
                Rates = tickerPrices.Rates
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewTickers_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Ctrl + C で選択済みの列をヘッダーと合わせてクリップボードにコピー
            if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control && e.Key == System.Windows.Input.Key.C)
            {
                const char Tab = '\t';
                var listView = (ListView)sender;
                var gridView = (GridView)listView.View;
                var stringBuilder = new System.Text.StringBuilder();

                foreach (var columns in gridView.Columns)
                {
                    stringBuilder.Append(columns.Header);
                    stringBuilder.Append(Tab);
                }
                stringBuilder.Append(Environment.NewLine);

                foreach (TickerPrices item in listView.SelectedItems)
                {
                    stringBuilder.Append(item.Symbol);
                    stringBuilder.Append(Tab);
                    stringBuilder.Append(item.BaseAsset);
                    stringBuilder.Append(Tab);
                    stringBuilder.Append(item.QuoteAsset);
                    stringBuilder.Append(Tab);
                    for(var i = 0; i < item.Prices.Count; ++i)
                    {
                        stringBuilder.Append(item.Prices[i].ToString("0.##############"));
                        stringBuilder.Append(Tab);
                        stringBuilder.Append(item.Rates[i].ToString("P"));
                        stringBuilder.Append(Tab);
                    }
                    stringBuilder.Append(Environment.NewLine);
                }

                System.Windows.Clipboard.SetText(stringBuilder.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTickers();

            //Log("[Auto Update] Download the latest pricing information.");
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

        #endregion
    }
}
