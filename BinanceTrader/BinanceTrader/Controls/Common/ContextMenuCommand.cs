using System;
using System.Windows.Input;

namespace BinanceTrader.Controls.Common
{

    /// <summary>
    /// コマンド
    /// </summary>
    public class ContextMenuCommand<T> : ICommand
    {
        /// <summary>
        /// 実行可能状態変更イベント
        /// </summary>
        #pragma warning disable 67
        public event EventHandler CanExecuteChanged;
        #pragma warning restore 67

        /// <summary>
        /// アクション
        /// </summary>
        private Action<T> action_;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="action">コマンド実行時アクション</param>
        public ContextMenuCommand(Action<T> action)
        {
            action_ = action;
        }

        /// <summary>
        /// 実行可能かを取得します
        /// </summary>
        /// <param name="parameter">パラメータ</param>
        /// <returns>実行可能な場合、trueを返します</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        /// <param name="parameter">パラメータ</param>
        public void Execute(object parameter)
        {
            if (action_ != null)
            {
                action_.Invoke((T)parameter);
            }
        }
    }
}
