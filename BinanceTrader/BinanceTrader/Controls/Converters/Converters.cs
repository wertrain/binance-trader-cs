using System;
using System.Windows.Data;

namespace BinanceTrader.Controls.Converters
{
    #region Prices & Rates converter

    /// <summary>
    /// 価格用コンバーター
    /// </summary>
    public class PriceToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((float)value).ToString("0.##############");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return float.Parse(value as string);
        }
    }

    /// <summary>
    /// 変化率用コンバーター
    /// </summary>
    public class RateToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var percent = ((float)value);

#if false
            var sb = new StringBuilder();
            sb.Append(percent.ToString("P"));

            if (percent == 0)
            {
                sb.Append("➙");
            }
            else if (percent > 0.0)
            {
                sb.Append("➚");
            }
            else
            {
                sb.Append("➘");
            }

            return sb.ToString();

#else
            return percent.ToString("P");
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return float.Parse(value as string);
        }
    }

    #endregion
}
