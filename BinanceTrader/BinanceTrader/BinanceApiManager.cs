namespace BinanceTrader
{
    /// <summary>
    /// Binance API 管理
    /// </summary>
    class BinanceApiManager
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static BinanceApiManager Instance { get; } = new BinanceApiManager();

        /// <summary>
        /// 
        /// </summary>
        public Binance.Api.Client Client { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private BinanceApiManager()
        {
            Client = new Binance.Api.Client();
        }
    }
}
