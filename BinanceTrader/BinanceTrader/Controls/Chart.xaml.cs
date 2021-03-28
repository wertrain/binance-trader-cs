using ScottPlot;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace BinanceTrader.Controls
{
    /// <summary>
    /// Chart.xaml の相互作用ロジック
    /// </summary>
    public partial class Chart : UserControl
    {
        /// <summary>
        /// チャート更新パラメータ
        /// </summary>
        public class ChartParam
        {
            public string Title { get; set; }

            public List<double> XS { get; set; } = new List<double>();

            public List<List<double>> YSList { get; set; } = new List<List<double>>();

            public List<string> Labels { get; set; } = new List<string>();

            public string YLabel { get; set; }
            public string XLabel { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly string ChartDefaultFontName = "Yu Gothic UI";

        public Chart()
        {
            InitializeComponent();

            /*
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
            _chart.plt.Title("Signal Plot サンプル", fontName: "Yu Gothic UI");

            //縦軸のタイトル
            _chart.plt.YLabel("縦軸のタイトル", fontName: "Yu Gothic UI", fontSize: 14);

            //横軸のタイトル
            _chart.plt.XLabel("横軸のタイトル", fontName: "Yu Gothic UI", fontSize: 14);

            //クライアント座標（左上が原点となるコントロール上の座標）で任意の文字列を表示
            _chart.plt.PlotAnnotation("任意の注意事項", 10, 200,
                fontSize: 24, fontName: "Yu Gothic UI", fontColor: System.Drawing.Color.Red, shadow: true,
                fill: true, fillColor: System.Drawing.Color.White, fillAlpha: 1, lineWidth: 2);

            //表示しているグラフのスケール座標に任意の文字列を表示
            _chart.plt.PlotText("スケール座標を使った文字列表示", 30, -20, fontSize: 16, bold: true, color: System.Drawing.Color.Magenta);

            //補助線の設定
            _chart.plt.Grid(xSpacing: 5, lineStyle: LineStyle.Dot, color: System.Drawing.Color.LightGray, lineWidth: 2);

            //凡例の表示
            _chart.plt.Legend(location: legendLocation.upperRight, fixedLineWidth: false);

            //グラフ表示の上限下限を設定
            _chart.plt.AxisBounds(minX: 5, maxX: 80, minY: double.NegativeInfinity, maxY: double.PositiveInfinity);

            _chart.Render();*/
        }

        public void UpdateChart(ChartParam param)
        {
            _chart.plt.Title(param.Title, fontName: ChartDefaultFontName);

            _chart.plt.Clear();

            //_chart.plt.Style(figBg: System.Drawing.Color.FromArgb(12, 12, 12));
            //_chart.plt.Style(dataBg: System.Drawing.Color.FromArgb(44, 44, 44));
            //_chart.plt.Style(label: System.Drawing.Color.FromArgb(255, 255, 255));
            //_chart.plt.Style(title: System.Drawing.Color.FromArgb(255, 255, 255));
            //_chart.plt.Style(tick: System.Drawing.Color.FromArgb(255, 255, 255));

            foreach (var (y, index) in param.YSList.Select((item, index) => (item, index)))
            {
                _chart.plt.PlotScatter(param.XS.ToArray(), y.ToArray(), label: param.Labels[index], lineStyle: LineStyle.Solid, lineWidth: 3.0);
            }

            _chart.plt.YLabel(param.YLabel, fontName: ChartDefaultFontName, fontSize: 14);
            _chart.plt.XLabel(param.XLabel, fontName: ChartDefaultFontName, fontSize: 14);

            _chart.plt.Grid(xSpacing: 5, lineStyle: LineStyle.Solid, color: System.Drawing.Color.LightGray, lineWidth: 1);

            _chart.Render();
        }
    }
}
