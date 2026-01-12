using System.Windows;
using System.Windows.Input;
using SimpleCheck.Services;

namespace SimpleCheck
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsService _settingsService;

        public SettingsWindow(SettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;
            
            // 加载当前状态
            if (_settingsService.Settings.CloseToTray)
                RadioToTray.IsChecked = true;
            else
                RadioExit.IsChecked = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _settingsService.Settings.CloseToTray = RadioToTray.IsChecked == true;
            _settingsService.SaveSettings();
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}