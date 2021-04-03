using BinanceTrader.Localize;
using System.Windows.Controls;

namespace BinanceTrader.Controls.Common
{
    public class ControlBase : UserControl
    {
        /// <summary>
        /// 情報メッセージを出力
        /// </summary>
        /// <param name="message"></param>
        protected void Log(string message)
        {
            Logging.Logger.Instance.Information(message.Localize());
        }

        /// <summary>
        /// エラーメッセージを出力
        /// </summary>
        /// <param name="message"></param>
        protected void Error(string message)
        {
            Logging.Logger.Instance.Error(message.Localize());
        }
    }
}
