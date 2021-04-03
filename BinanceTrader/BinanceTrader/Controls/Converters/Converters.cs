using BinanceTrader.Localize;
using System;
using System.Drawing;
using System.Windows.Data;
using System.Windows.Media;

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

    /// <summary>
    /// 損益数値からカラーに変換するコンバーター
    /// </summary>
    public class ColorOfProfitAndLoss : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var floatValue = (float)value;

            if (floatValue == 0)
            {
                return new SolidColorBrush(Colors.DarkSlateGray);
            }
            else if (floatValue > 0.0)
            {
                return new SolidColorBrush(Colors.BlueViolet);
            }
            else
            {
                return new SolidColorBrush(Colors.OrangeRed);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    /// <summary>
    /// ログタイプをテキストに変換
    /// </summary>
    public class LogTypesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((Logging.Logger.LogInfo.Types)value)
            {
                case Logging.Logger.LogInfo.Types.Information:
                    return "Information Icon".Localize();

                case Logging.Logger.LogInfo.Types.Error:
                    return "Error Icon".Localize();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
