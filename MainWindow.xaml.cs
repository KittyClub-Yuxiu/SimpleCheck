using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SimpleCheck.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls;

namespace SimpleCheck
{
    public partial class MainWindow : Window
    {
        private bool _isExit = false;
        private TaskbarIcon? _taskbarIcon;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            try 
            {
                SetupNotifyIcon();
                SetupDateCheckTimer();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化报错: {ex.Message}");
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // 修改关闭逻辑：根据设置决定行为
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.SettingsService.Settings.CloseToTray)
                {
                    Hide(); // 最小化到托盘
                }
                else
                {
                    ExitApplication(); // 直接退出
                }
            }
            else
            {
                Hide(); // 默认行为
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void SetupNotifyIcon()
        {
            try 
            {
                _taskbarIcon = new TaskbarIcon
                {
                    ToolTipText = "Simple Check",
                    IconSource = new BitmapImage(new Uri("pack://application:,,,/Resources/appicon.ico")),
                    Visibility = Visibility.Visible
                };

                _taskbarIcon.TrayLeftMouseDown += (sender, e) => ShowWindow();

                var contextMenu = new ContextMenu();
                var showMenuItem = new MenuItem { Header = "显示窗口" };
                showMenuItem.Click += (sender, e) => ShowWindow();
                contextMenu.Items.Add(showMenuItem);
                contextMenu.Items.Add(new Separator());
                var exitMenuItem = new MenuItem { Header = "退出" };
                exitMenuItem.Click += (sender, e) => ExitApplication();
                contextMenu.Items.Add(exitMenuItem);
                _taskbarIcon.ContextMenu = contextMenu;
            }
            catch { }
        }

        private void ExitApplication()
        {
            _isExit = true;
            if (_taskbarIcon != null) _taskbarIcon.Dispose();
            Application.Current.Shutdown();
        }

        private void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!_isExit)
            {
                // 如果不是显式退出，拦截关闭事件
                e.Cancel = true;
                
                // 复用关闭按钮的逻辑
                Close_Click(this, new RoutedEventArgs());
            }
            else
            {
                if (_taskbarIcon != null) _taskbarIcon.Dispose();
                base.OnClosing(e);
            }
        }

        private void SetupDateCheckTimer()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            timer.Tick += (sender, e) => 
            {
                 if (DataContext is MainViewModel viewModel) viewModel.RefreshDailyTasks();
            };
            timer.Start();
        }
    }
}