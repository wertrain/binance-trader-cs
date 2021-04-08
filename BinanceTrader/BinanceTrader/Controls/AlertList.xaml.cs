using BinanceTrader.Controls.Common;
using System.Collections.ObjectModel;

namespace BinanceTrader.Controls
{
    /// <summary>
    /// AlertList.xaml の相互作用ロジック
    /// </summary>
    public partial class AlertList : ControlBase
    {
        /// <summary>
        /// アラート情報
        /// </summary>
        public class AlertInfo
        {

        }

        /// <summary>
        /// アラート情報
        /// </summary>
        public ObservableCollection<AlertInfo> Alerts;

        public AlertList()
        {
            InitializeComponent();
        }
    }
}
