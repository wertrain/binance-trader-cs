using BinanceTrader.Localize;
using BinanceTrader.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceTrader.Services.Alerts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAlert
    {
        /// <summary>
        /// 
        /// </summary>
        void Check();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetSummaryText();

        /// <summary>
        /// 
        /// </summary>
        bool Enabled { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PriceRateIncreaseAlert : IAlert
    {
        /// <summary>
        /// 
        /// </summary>
        public float Percent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSummaryText()
        {
            return string.Format("Notify when {0} rise within {1} minutes".Localize(), Percent.ToString("P"), 3);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public PriceRateIncreaseAlert(float percent) => Percent = percent * 0.01f;

        /// <summary>
        /// 
        /// </summary>
        public void Check()
        {
            var jsonPricesList = new List<List<PricePair>>();

            // 現在の価格を Binance API を使用して取得
            {
                var response = TraderApiManager.Instance.Cache.GetAllTickers();
                jsonPricesList.Add(response.ToObject());
            }

            // 過去の価格をサーバーから取得
            try
            {
                var response = TraderApiManager.Instance.Cache.GetPastTimeTickers();
                jsonPricesList.AddRange(response.ToObject());
            }
            catch
            {
                return;
            }

            var currentPrices = jsonPricesList.FirstOrDefault();

            foreach (var prices in currentPrices)
            {
                var targetPrices = jsonPricesList[1];
                var target = targetPrices.Find(p => p.Symbol == prices.Symbol);

                if (float.TryParse(prices.Price, out var cp) && float.TryParse(target.Price, out var tp))
                {
                    var percent = (cp - tp) / tp;

                    if (percent >= Percent)
                    {
                        Logging.Logger.Instance.Alert(string.Format("{0} is up {1}.".Localize(), prices.Symbol, percent.ToString("P")), prices);
                    }
                }
            }
        }
    }
}

namespace BinanceTrader.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AlertService : ServiceBase
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Alerts.IAlert> Alerts { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public AlertService()
        {
            Alerts = new List<Alerts.IAlert>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alert"></param>
        public void AddAlert(Alerts.IAlert alert)
        {
            Alerts.Add(alert);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Create()
        {
            TraderDispatcherTimer.Instance.Tick += Timer_Tick;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach(var alert in Alerts)
            {
                if (alert.Enabled)
                {
                    alert.Check();
                }
            }
        }
    }
}
