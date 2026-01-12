using System;
using System.Globalization;
using System.Windows.Data;

namespace SimpleCheck.Converters
{
    // 这是一个没有任何依赖属性的纯净版转换器，绝对安全
    public class TimeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "选择时间";
            if (value is DateTime time) return time.ToString("HH:mm");
            return value?.ToString() ?? "选择时间";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}