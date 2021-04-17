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
        /// 値のコンバーター
        /// </summary>
        Func<object, T> converter_;

        /// <summary>
        /// アクション
        /// </summary>
        private Action<object> execute_;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="action">コマンド実行時アクション</param>
        public ContextMenuCommand(Action<T> action)
        {
            action_ = action;
            execute_ = DoExecute;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="action">コマンド実行時アクション</param>
        public ContextMenuCommand(Action<T> action, Func<object, T> converter)
        {
            action_ = action;
            converter_ = converter;
            execute_ = DoConvertAndExecute;
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
        /// コマンドを実行
        /// </summary>
        /// <param name="parameter">パラメータ</param>
        public void Execute(object parameter)
        {
            execute_(parameter);
        }

        /// <summary>
        /// コマンドの実行処理
        /// </summary>
        /// <param name="parameter"></param>
        private void DoExecute(object parameter)
        {
            action_?.Invoke((T)parameter);
        }

        /// <summary>
        /// 値をコンバートしてコマンドを実行
        /// </summary>
        /// <param name="parameter"></param>
        private void DoConvertAndExecute(object parameter)
        {
            action_?.Invoke(converter_.Invoke(parameter));
        }
    }
}
