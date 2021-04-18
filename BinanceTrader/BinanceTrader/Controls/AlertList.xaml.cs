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
            /// アラートインターフェース
            /// </summary>
            private Services.Alerts.IAlert Alert;

            /// <summary>
            /// 
            /// </summary>
            public bool Enabled
            {
                get { return Alert.Enabled; }
                set { Alert.Enabled = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            public string Conditions => Alert.GetSummaryText();

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="alert"></param>
            public AlertInfo(Services.Alerts.IAlert alert) => Alert = alert;
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
                    Alerts.Add(new AlertInfo(alert));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CheckBoxAll_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach(var alert in Alerts)
            {
                alert.Enabled = true;
            }
            _listViewAlerts.Items.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CheckBoxAll_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var alert in Alerts)
            {
                alert.Enabled = false;
            }
            _listViewAlerts.Items.Refresh();
        }
    }
}
