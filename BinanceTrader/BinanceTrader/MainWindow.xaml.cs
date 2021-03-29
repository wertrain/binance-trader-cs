using BinanceTrader.Controls;
using BinanceTrader.Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static BinanceTrader.Controls.TickerList;

namespace BinanceTrader
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _tickerList.SelectionChanged += TickerList_SelectionChanged;
            _tickerList.OnSelectVirtualPurchase += TickerList_OnSelectVirtualPurchase;
            _virtualPurchases.SelectionChanged += VirtualPurchases_SelectionChanged;
        }

        /// <summary>
        /// メニューアイテム「テーマ」の子アイテムがクリックされた時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemToolTheme_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            foreach (MenuItem sibling in (menuItem.Parent as MenuItem).Items)
            {
                sibling.IsChecked = menuItem == sibling;
            }
            _dockingManager.Theme = menuItem.Tag as Xceed.Wpf.AvalonDock.Themes.Theme;
        }

        /// <summary>
        /// ウィンドウが開くときのイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(var info in Settings.Instance.VirtualPurchases)
            {
                _virtualPurchases.AddPurchaseInfo(new VirtualPurchaseList.PurchaseInfo()
                {
                    Symbol = info.Symbol,
                    BaseAsset = info.BaseAsset,
                    QuoteAsset = info.QuoteAsset,
                    PurchaseDate = info.PurchaseDate,
                    PurchasePrice = info.PurchasePrice
                });
            }
        }

        /// <summary>
        /// ウィンドウが閉じるときのイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Instance.VirtualPurchases.Clear();
            foreach (var info in _virtualPurchases.Purchases)
            {
                Settings.Instance.VirtualPurchases.Add(new Settings.VirtualPurchaseInfo()
                {
                    Symbol = info.Symbol,
                    BaseAsset = info.BaseAsset,
                    QuoteAsset = info.QuoteAsset,
                    PurchaseDate = info.PurchaseDate,
                    PurchasePrice = info.PurchasePrice
                });
            }
            Settings.Instance.Save();
        }

        /// <summary>
        /// アプリケーションの終了
        /// </summary>
        private void Shutdown()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 閉じるコマンド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseCommand(object sender, ExecutedRoutedEventArgs e) => Shutdown();

        /// <summary>
        /// 銘柄リストの選択が変更された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TickerList_SelectionChanged(object sender, TickerSelectionChangedEventArgs e)
        {

            //{
            //    var param = new Controls.Chart.ChartParam();
            //    param.Title = e.Symbol;
            //    param.XS = Enumerable.Range(0, e.Prices.Count).Select(i => (double)i).ToList();
            //    param.YSList.Add(e.Prices.Select<float, double>(i => i).ToList());
            //    param.Labels.Add("Price");
            //    _chartPrices.UpdateChart(param);
            //}

            {
                var param = new Controls.Chart.ChartParam();
                param.Title = e.Symbol;
                param.XS = Enumerable.Range(0, e.Prices.Count).Select(i => (double)i).ToList();
                param.YSList.Add(e.Rates.Select<float, double>(i => i).ToList());
                param.Labels.Add("Rate Of Change");
                _chartRates.UpdateChart(param);
            }
        }

        /// <summary>
        /// 仮想購入が選択された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TickerList_OnSelectVirtualPurchase(object sender, VirtualPurchaseEventArgs e)
        {
            _virtualPurchases.AddPurchaseInfo(new VirtualPurchaseList.PurchaseInfo()
            {
                Symbol = e.Symbol,
                BaseAsset = e.BaseAsset,
                QuoteAsset = e.QuoteAsset,
                PurchasePrice = e.Price,
                CurrentPrice = e.Price,
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VirtualPurchases_SelectionChanged(object sender, VirtualPurchaseList.VirtualPurchaseSelectionChangedEventArgs e)
        {
            _tickerList.SelectSymbol(e.Symbol);
        }
    }
}
