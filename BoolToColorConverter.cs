using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SimpleCheck
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted && parameter is string colorString)
            {
                string[] colors = colorString.Split(';');
                if (colors.Length >= 2)
                {
                    string completedColor = colors[0];
                    string notCompletedColor = colors[1];
                    
                    if (isCompleted)
                    {
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(completedColor));
                    }
                    else
                    {
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(notCompletedColor));
                    }
                }
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#212529"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}