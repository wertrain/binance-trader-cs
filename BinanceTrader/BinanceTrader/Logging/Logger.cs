using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceTrader.Logging
{
    /// <summary>
    /// ログ管理
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// ログ情報
        /// </summary>
        public class LogInfo
        {
            public enum Types
            {
                Information,
                Alert,
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
        /// インスタンス
        /// </summary>
        public static Logger Instance { get; } = new Logger();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Logger()
        {
            Logs = new List<LogInfo>();
        }

        /// <summary>
        /// 情報メッセージを出力
        /// </summary>
        /// <param name="message"></param>
        public void Information(string message)
        {
            Logging(LogInfo.Types.Information, message);
        }

        /// <summary>
        /// アラートメッセージを出力
        /// </summary>
        /// <param name="message"></param>
        public void Alert(string message)
        {
            Logging(LogInfo.Types.Alert, message);
        }

        /// <summary>
        /// エラーメッセージを出力
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            Logging(LogInfo.Types.Error, message);
        }

        /// <summary>
        /// すべてのログを消去
        /// </summary>
        public void Clear()
        {
            Logs.Clear();
        }

        /// <summary>
        /// ログ
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="message"></param>
        protected void Logging(LogInfo.Types logType, string message)
        {
            Logs.Insert(0, new LogInfo()
            {
                LogType = logType,
                DateTime = DateTime.Now,
                Message = message
            });

            OnLogged?.Invoke(this, new LoggedEventArgs(Logs));
        }

        /// <summary>
        /// ログ情報
        /// </summary>
        public List<LogInfo> Logs { get; }

        /// <summary>
        /// ログ記録イベントの引数
        /// </summary>
        public class LoggedEventArgs
        {
            public List<LogInfo> Logs;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public LoggedEventArgs(List<LogInfo> logs) => Logs = logs;
        }

        /// <summary>
        /// ログが登録された時のイベント
        /// </summary>
        public EventHandler<LoggedEventArgs> OnLogged;
    }
}
