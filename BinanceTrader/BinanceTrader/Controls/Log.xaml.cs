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

            _listViewLogs.DataContext = this;
            _listViewLogs.ItemsSource = Logs;
        }
    }
}
