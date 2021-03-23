using BinanceTrader.Localize;
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
            {
                var param = new Controls.Chart.ChartParam();
                param.Title = e.Symbol;
                param.XS = Enumerable.Range(0, e.Prices.Count).Select(i => (double)i).ToList();
                param.YSList.Add(e.Prices.Select<float, double>(i => i).ToList());
                param.Labels.Add("Price");
                _chartPrices.UpdateChart(param);
            }

            {
                var param = new Controls.Chart.ChartParam();
                param.Title = e.Symbol;
                param.XS = Enumerable.Range(0, e.Prices.Count).Select(i => (double)i).ToList();
                param.YSList.Add(e.Rates.Select<float, double>(i => i).ToList());
                param.Labels.Add("Rate Of Change");
                _chartRates.UpdateChart(param);
            }
        }
    }
}
