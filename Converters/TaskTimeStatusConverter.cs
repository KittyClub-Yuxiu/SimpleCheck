using System;
using System.Globalization;
using System.Windows.Data;

namespace SimpleCheck.Converters
{
    public class TaskTimeStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // 获取开始时间和结束时间
            // 注意：这里假设绑定的顺序是：Binding StartTime, Binding EndTime
            var startTime = values[0] as DateTime?;
            var endTime = values[1] as DateTime?;

            // 1. 缺少时间 -> 显示“本日”
            if (startTime == null && endTime == null)
            {
                return "本日";
            }

            // 2. 只有开始时间 -> 显示“开始于 HH:mm”
            if (startTime != null && endTime == null)
            {
                return $"开始于 {startTime.Value:HH:mm}";
            }

            // 3. 只有结束时间 -> 显示“结束于 HH:mm”
            if (startTime == null && endTime != null)
            {
                return $"结束于 {endTime.Value:HH:mm}";
            }

            // 4. 都有 -> 显示“HH:mm - HH:mm”
            if (startTime != null && endTime != null)
            {
                return $"{startTime.Value:HH:mm} - {endTime.Value:HH:mm}";
            }

            return "本日";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}