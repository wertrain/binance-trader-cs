using BinanceTrader.Controls.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
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

namespace BinanceTrader.Controls
{
    /// <summary>
    /// VirtualPurchaseList.xaml の相互作用ロジック
    /// </summary>
    public partial class VirtualPurchaseList : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public class PurchaseInfo
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

            /// <summary>
            /// 現在価格
            /// </summary>
            public float CurrentPrice { get; set; }

            /// <summary>
            /// 損益
            /// </summary>
            public float ProfitAndLossPrice { get; set; }

            /// <summary>
            /// 損益
            /// </summary>
            public float ProfitAndLossRate { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public PurchaseInfo()
            {
                PurchaseDate = DateTime.Now;
            }
        }

        /// <summary>
        /// 仮想購入情報
        /// </summary>
        public ObservableCollection<PurchaseInfo> Purchases;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VirtualPurchaseList()
        {
            InitializeComponent();

            Purchases = new ObservableCollection<PurchaseInfo>();

            RemoveVirtualPurchaseCommand = new ContextMenuCommand<PurchaseInfo>(SelectRemoveFromList);

            _listViewPurchases.DataContext = this;
            _listViewPurchases.ItemsSource = Purchases;

            TraderDispatcherTimer.Instance.Tick += Timer_Tick;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdatePurchaseList()
        {
            var response = TraderApiManager.Instance.Cache.GetAllTickers();
            var list = response.ToObject();

            foreach (var purchase in Purchases)
            {
                var p = list.Find(n => n.Symbol == purchase.Symbol);

                if (float.TryParse(p?.Price, out var price))
                {
                    purchase.CurrentPrice = price;
                    purchase.ProfitAndLossPrice = purchase.CurrentPrice - purchase.PurchasePrice;
                    purchase.ProfitAndLossRate = purchase.ProfitAndLossPrice / purchase.PurchasePrice;
                }
            }
            _listViewPurchases.Items.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public void AddPurchaseInfo(PurchaseInfo info)
        {
            Purchases.Add(info);

            UpdatePurchaseList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewPurchases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(PurchaseInfo info in e.AddedItems)
            {
                SelectionChanged?.Invoke(this, new VirtualPurchaseSelectionChangedEventArgs(info.Symbol));
            }
        }

        /// <summary>
        /// リストから削除が選択された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectRemoveFromList(PurchaseInfo item)
        {
            Purchases.Remove(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdatePurchaseList();
        }

        #region イベント関連

        /// <summary>
        /// 仮想購入が選択されたときのイベント引数
        /// </summary>
        public class VirtualPurchaseSelectionChangedEventArgs
        {
            /// <summary>
            /// 銘柄名
            /// </summary>
            public string Symbol { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="symbol"></param>
            public VirtualPurchaseSelectionChangedEventArgs(string symbol) => Symbol = symbol;
        }

        /// <summary>
        /// リストから銘柄を選択したときのイベント
        /// </summary>
        public EventHandler<VirtualPurchaseSelectionChangedEventArgs> SelectionChanged;

        #endregion

        #region コマンド関連

        /// <summary>
        /// リストアイテム選択時のコマンド
        /// </summary>
        public ContextMenuCommand<PurchaseInfo> RemoveVirtualPurchaseCommand { get; set; }

        #endregion
    }
}
