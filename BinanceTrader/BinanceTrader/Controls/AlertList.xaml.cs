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
            /// <summary>
            /// 
            /// </summary>
            public bool Enabled { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public string Conditions { get; set; }
        }

        /// <summary>
        /// アラート情報
        /// </summary>
        public ObservableCollection<AlertInfo> Alerts;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AlertList()
        {
            InitializeComponent();

            Alerts = new ObservableCollection<AlertInfo>();

            _listViewAlerts.DataContext = this;
            _listViewAlerts.ItemsSource = Alerts;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateAlertList()
        {
            var alertService = Services.ServiceManager.Instance.GetService<Services.AlertService>();

            if (alertService != null)
            {
                foreach (var alert in alertService.Alerts)
                {
                    Alerts.Add(new AlertInfo()
                    {
                        Enabled = alert.Enabled,
                        Conditions = alert.GetSummaryText()
                    });
                }
            }
        }
    }
}
