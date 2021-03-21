using System;
using System.Collections.Generic;
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

namespace BinanceTrader
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// メニューアイテム「テーマ」の子アイテムがクリックされた時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemToolTheme_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            foreach (MenuItem sibling in (menuItem.Parent as MenuItem).Items)
            {
                sibling.IsChecked = menuItem == sibling;
            }
            _dockingManager.Theme = menuItem.Tag as Xceed.Wpf.AvalonDock.Themes.Theme;
        }

        /// <summary>
        /// アプリケーションの終了
        /// </summary>
        private void Shutdown()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 閉じるコマンド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseCommand(object sender, ExecutedRoutedEventArgs e) => Shutdown();
    }
}
