using System;
using System.Threading.Tasks;
using System.Windows;
using SimpleCheck.ViewModels;

namespace SimpleCheck;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e); // 必须调用基类启动
            
            try
            {
                // 显示加载页
                var splashScreen = new SplashScreen();
                splashScreen.Show();
                
                // 先创建主窗口但不显示
                var mainWindow = new MainWindow();
                
                // 强制设置主窗口指针
                this.MainWindow = mainWindow;
                
                // 直接创建ViewModel并订阅加载完成事件
                var viewModel = new MainViewModel();
                
                // 订阅加载完成事件
                viewModel.LoadingCompleted += () =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        // 恢复主窗口的任务栏显示
                        mainWindow.ShowInTaskbar = true;
                        mainWindow.Show();
                        mainWindow.Activate();
                        
                        // 暴力置顶一下，防止被 IDE 遮挡
                        mainWindow.Topmost = true;
                        mainWindow.Topmost = false;
                        
                        // 关闭加载页
                        splashScreen.Close();
                    });
                };
                
                // 将ViewModel设置为DataContext
                mainWindow.DataContext = viewModel;
                
                // 确保用户可以通过任务管理器结束进程
                // 设置主窗口属性，但不立即显示
                mainWindow.ShowInTaskbar = false;
                mainWindow.Visibility = Visibility.Hidden;
                mainWindow.ShowActivated = false;
                
                // 显示主窗口但保持隐藏，直到加载完成
                // 这确保了窗口句柄被创建，用户可以通过任务管理器结束进程
                mainWindow.Show();
                mainWindow.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动严重错误: " + ex.ToString());
            }
        }
}