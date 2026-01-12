using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;       // 必须引用这个，否则 Binding.DoNothing 会报错
using SimpleCheck.Models;

namespace SimpleCheck
{
    public class TaskTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskType taskType)
            {
                // 确保这里的字符串和你的 ComboBox/RadioButton 显示的一致
                return taskType switch
                {
                    TaskType.OneTime => "单次",
                    TaskType.Daily => "每日",
                    _ => "未知"
                };
            }
            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // --- 修复重点：绝对不能抛出异常 ---
            var str = value as string;
            
            if (str == "单次") return TaskType.OneTime;
            if (str == "每日") return TaskType.Daily;
            
            // 如果转换失败，告诉 WPF "什么都不做"，而不是报错
            return Binding.DoNothing; 
        }
    }
}