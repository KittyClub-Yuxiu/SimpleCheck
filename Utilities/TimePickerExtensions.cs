using System;
using System.Windows;
using System.Windows.Controls;

namespace SimpleCheck.Utilities
{
    public class TimePickerExtensions
    {
        // 选中的完整时间
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.RegisterAttached("SelectedTime", typeof(DateTime?), typeof(TimePickerExtensions),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTimeChanged));

        public static DateTime? GetSelectedTime(DependencyObject obj) => (DateTime?)obj.GetValue(SelectedTimeProperty);
        public static void SetSelectedTime(DependencyObject obj, DateTime? value) => obj.SetValue(SelectedTimeProperty, value);

        // 选中的小时
        public static readonly DependencyProperty SelectedHourProperty =
            DependencyProperty.RegisterAttached("SelectedHour", typeof(int), typeof(TimePickerExtensions),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedHourChanged));

        public static int GetSelectedHour(DependencyObject obj) => (int)obj.GetValue(SelectedHourProperty);
        public static void SetSelectedHour(DependencyObject obj, int value) => obj.SetValue(SelectedHourProperty, value);

        // 选中的分钟
        public static readonly DependencyProperty SelectedMinuteProperty =
            DependencyProperty.RegisterAttached("SelectedMinute", typeof(int), typeof(TimePickerExtensions),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedMinuteChanged));

        public static int GetSelectedMinute(DependencyObject obj) => (int)obj.GetValue(SelectedMinuteProperty);
        public static void SetSelectedMinute(DependencyObject obj, int value) => obj.SetValue(SelectedMinuteProperty, value);

        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatePicker datePicker && e.NewValue is DateTime time)
            {
                datePicker.SetValue(SelectedHourProperty, time.Hour);
                datePicker.SetValue(SelectedMinuteProperty, time.Minute);
            }
        }

        private static void OnSelectedHourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatePicker datePicker) UpdateSelectedTime(datePicker);
        }

        private static void OnSelectedMinuteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatePicker datePicker) UpdateSelectedTime(datePicker);
        }

        private static void UpdateSelectedTime(DatePicker datePicker)
        {
            var current = GetSelectedTime(datePicker) ?? DateTime.Now.Date;
            var hour = GetSelectedHour(datePicker);
            var minute = GetSelectedMinute(datePicker);
            
            var newTime = new DateTime(current.Year, current.Month, current.Day, hour, minute, 0);
            
            if (GetSelectedTime(datePicker) != newTime)
            {
                SetSelectedTime(datePicker, newTime);
            }
        }
    }
}