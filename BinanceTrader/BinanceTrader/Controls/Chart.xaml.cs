using ScottPlot;
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

namespace BinanceTrader.Controls
{
    /// <summary>
    /// Chart.xaml の相互作用ロジック
    /// </summary>
    public partial class Chart : UserControl
    {
        public Chart()
        {
            InitializeComponent();

            Random rand = new Random(0);
            var xs = Enumerable.Range(0, 100).Select(i => (double)i).ToArray();
            var ys1 = DataGen.RandomWalk(rand, 100);
            var ys2 = DataGen.Sin(100, mult: 25);
            var ys3 = DataGen.RandomWalk(rand, 100);
            var ys = new double[][] { ys1, ys2, ys3 };

            _chart.plt.Title("test");
            for (int i = 0; i < 3; i++)
            {
                _chart.plt.PlotScatter(xs, ys[i], label: i.ToString());
            }

            //任意のドットを描画
            _chart.plt.PlotPoint(50, 20, color: System.Drawing.Color.Magenta, markerSize: 10);

            //グラフのタイトルを表示
            _chart.plt.Title("Signal Plot サンプル", fontName: "游ゴシック");

            //縦軸のタイトル
            _chart.plt.YLabel("縦軸のタイトル", fontName: "游ゴシック", fontSize: 14);

            //横軸のタイトル
            _chart.plt.XLabel("横軸のタイトル", fontName: "游ゴシック", fontSize: 14);

            //クライアント座標（左上が原点となるコントロール上の座標）で任意の文字列を表示
            _chart.plt.PlotAnnotation("任意の注意事項", 10, 200,
                fontSize: 24, fontName: "游ゴシック", fontColor: System.Drawing.Color.Red, shadow: true,
                fill: true, fillColor: System.Drawing.Color.White, fillAlpha: 1, lineWidth: 2);

            //表示しているグラフのスケール座標に任意の文字列を表示
            _chart.plt.PlotText("スケール座標を使った文字列表示", 30, -20, fontSize: 16, bold: true, color: System.Drawing.Color.Magenta);

            //補助線の設定
            _chart.plt.Grid(xSpacing: 5, lineStyle: LineStyle.Dot, color: System.Drawing.Color.LightGray, lineWidth: 2);

            //凡例の表示
            _chart.plt.Legend(location: legendLocation.upperRight, fixedLineWidth: false);

            //グラフ表示の上限下限を設定
            _chart.plt.AxisBounds(minX: 5, maxX: 80, minY: double.NegativeInfinity, maxY: double.PositiveInfinity);

            _chart.Render();
        }

        public void UpdateChart()
        {

        }
    }
}
