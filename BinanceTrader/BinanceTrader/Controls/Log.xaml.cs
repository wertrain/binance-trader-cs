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
        /// ログ情報
        /// </summary>
        public class LogMessage
        {
            public enum Types
            {
                Information,
                Error
            };

            /// <summary>
            /// 
            /// </summary>
            public Types LogType { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime DateTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Message { get; set; }
        }

        /// <summary>
        /// ログ情報
        /// </summary>
        private ObservableCollection<LogMessage> Logs;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Log()
        {
            InitializeComponent();

            Logs = new ObservableCollection<LogMessage>();

            Logs.Add(new LogMessage()
            {
                DateTime = DateTime.Now,
                Message = "test"
            });

            _listViewLogs.DataContext = this;
            _listViewLogs.ItemsSource = Logs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSaveFile_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.FileName = "logs.txt";
            sfd.Filter = "テキストファイル(*.txt)|*.txt";
            if (sfd.ShowDialog() ?? false)
            {
                foreach (var log in Logs)
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
            var checkedButtons = new List<LogMessage.Types>();
            if (_buttonFilterInformation.IsChecked ?? false) checkedButtons.Add(LogMessage.Types.Information);
            if (_buttonFilterError.IsChecked ?? false) checkedButtons.Add(LogMessage.Types.Error);

            var filtered = Logs.Where(x => checkedButtons.Contains(x.LogType)).ToList();
            _listViewLogs.ItemsSource = filtered;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            Logs.Clear();
            _listViewLogs.ItemsSource = Logs;
        }
    }
}
