using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BinanceTrader.Controls
{
    /// <summary>
    /// Log.xaml の相互作用ロジック
    /// </summary>
    public partial class Log : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Log()
        {
            InitializeComponent();

            _listViewLogs.DataContext = this;
            _listViewLogs.ItemsSource = Logging.Logger.Instance.Logs;

            Logging.Logger.Instance.OnLogged += Logger_OnLogged;
        }

        /// <summary>
        /// ログを更新して反映する
        /// </summary>
        private void UpdateLog()
        {
            var checkedButtons = new List<Logging.Logger.LogInfo.Types>();
            if (_buttonFilterInformation.IsChecked ?? false) checkedButtons.Add(Logging.Logger.LogInfo.Types.Information);
            if (_buttonFilterError.IsChecked ?? false) checkedButtons.Add(Logging.Logger.LogInfo.Types.Error);

            var filtered = Logging.Logger.Instance.Logs.Where(x => checkedButtons.Contains(x.LogType)).ToList();
            _listViewLogs.ItemsSource = filtered;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSaveFile_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "log.txt",
                Filter = "テキストファイル(*.txt)|*.txt"
            };
            if (sfd.ShowDialog() ?? false)
            {
                var logs = new StringBuilder();
                foreach (var log in Logging.Logger.Instance.Logs)
                {
                    logs.AppendFormat("{0}\t{1}\t{2}", log.LogType, log.DateTime, log.Message);
                    logs.Append(Environment.NewLine);
                }
                using (var stream = sfd.OpenFile())
                using (var writer = new System.IO.StreamWriter(stream))
                {
                    writer.Write(logs.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            UpdateLog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            Logging.Logger.Instance.Clear();
            _listViewLogs.ItemsSource = Logging.Logger.Instance.Logs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logger_OnLogged(object sender, Logging.Logger.LoggedEventArgs e)
        {
            UpdateLog();
        }
    }
}
