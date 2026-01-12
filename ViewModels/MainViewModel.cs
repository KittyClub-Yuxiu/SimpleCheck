using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SimpleCheck.Models;
using SimpleCheck.Services;
using SimpleCheck.Utilities;

namespace SimpleCheck.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly TodoDataService _todoDataService;
        private readonly QuoteService _quoteService;
        
        // 新增 SettingsService
        public readonly SettingsService SettingsService; 
        
        // 加载跟踪
        private int _loadingOperations = 0;
        private int _completedOperations = 0;
        
        public ObservableCollection<TodoItem> TodoItems { get; set; }

        private string? _dailyQuote;
        public string? DailyQuote 
        {
            get => _dailyQuote;
            set => SetProperty(ref _dailyQuote, value);
        }

        public ICommand DeleteTaskCommand { get; } 
        public ICommand AddTaskCommand { get; }
        public ICommand OpenTaskEditorCommand { get; }
        public ICommand ToggleTaskCommand { get; }
        public ICommand ClearCompletedCommand { get; }
        public ICommand RestoreWindowCommand { get; }
        public ICommand ExitApplicationCommand { get; }
        
        // 新增命令
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenAboutCommand { get; }

        public event Action LoadingCompleted;

        public MainViewModel()
        {
            _todoDataService = new TodoDataService();
            _quoteService = new QuoteService();
            SettingsService = new SettingsService(); // 初始化设置服务
            
            TodoItems = new ObservableCollection<TodoItem>();
            
            AddTaskCommand = new RelayCommand(OpenTaskEditor);
            OpenTaskEditorCommand = new RelayCommand(OpenTaskEditor);
            DeleteTaskCommand = new RelayCommand<TodoItem>(DeleteTask);
            ToggleTaskCommand = new RelayCommand<TodoItem>(ToggleTask);
            ClearCompletedCommand = new RelayCommand(ClearCompletedTasks);
            RestoreWindowCommand = new RelayCommand(RestoreWindow);
            ExitApplicationCommand = new RelayCommand(ExitApplication);
            
            // 绑定新命令
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            OpenAboutCommand = new RelayCommand(OpenAbout);

            LoadDataAsync();
            LoadDailyQuote();
        }
        
        // 打开设置页
        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow(SettingsService)
            {
                Owner = Application.Current.MainWindow
            };
            settingsWindow.ShowDialog();
        }

        // 打开关于页
        private void OpenAbout()
        {
            var aboutWindow = new AboutWindow
            {
                Owner = Application.Current.MainWindow
            };
            aboutWindow.ShowDialog();
        }

        // ... (其他原有方法 LoadDataAsync, CheckLoadingCompletion, OpenTaskEditor 等保持不变) ...
        
        private async void LoadDataAsync()
        {
            _loadingOperations++;
            await Task.Run(() =>
            {
                try
                {
                    var todos = _todoDataService.LoadTodos();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var item in todos) TodoItems.Add(item);
                        RefreshDailyTasks();
                    });
                }
                catch { }
            });
            _completedOperations++;
            CheckLoadingCompletion();
        }

        private async void LoadDailyQuote()
        {
            _loadingOperations++;
            try
            {
                DailyQuote = await Task.Run(async () => await _quoteService.GetDailyQuoteAsync());
            }
            catch (Exception)
            {
                DailyQuote = _quoteService.GetDailyQuote();
            }
            _completedOperations++;
            CheckLoadingCompletion();
        }

        private void CheckLoadingCompletion()
        {
            if (_completedOperations >= _loadingOperations)
            {
                LoadingCompleted?.Invoke();
            }
        }

        private void OpenTaskEditor()
        {
            try 
            {
                var taskEditorVM = new TaskEditorViewModel();
                var taskEditorWindow = new TaskEditorWindow
                {
                    DataContext = taskEditorVM,
                    Owner = Application.Current.MainWindow
                };
                
                taskEditorVM.TaskSaved += (task) =>
                {
                    TodoItems.Add(task);
                    SaveTodos();
                    taskEditorWindow.Close();
                };
                
                taskEditorVM.TaskCancelled += () => taskEditorWindow.Close();
                
                taskEditorWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                 MessageBox.Show($"打开任务编辑器失败: {ex.Message}");
            }
        }

        private void DeleteTask(TodoItem task)
        {
            if (task != null)
            {
                TodoItems.Remove(task);
                SaveTodos();
            }
        }

        private void ToggleTask(TodoItem task)
        {
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                task.LastUpdatedTime = DateTime.Now;
                SaveTodos();
            }
        }

        private void ClearCompletedTasks()
        {
            var completedTasks = TodoItems.Where(t => t.IsCompleted).ToList();
            foreach (var task in completedTasks) TodoItems.Remove(task);
            SaveTodos();
        }

        public void RefreshDailyTasks()
        {
            var today = DateTime.Now.Date;
            bool needSave = false;
            foreach (var task in TodoItems)
            {
                if (task.Type == TaskType.Daily && task.LastUpdatedTime.Date < today)
                {
                    task.IsCompleted = false;
                    task.LastUpdatedTime = DateTime.Now;
                    needSave = true;
                }
            }
            if (needSave) SaveTodos();
        }

        private void SaveTodos()
        {
            try { _todoDataService.SaveTodos(TodoItems.ToList()); } catch { }
        }

        private void RestoreWindow()
        {
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Show();
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                Application.Current.MainWindow.Activate();
            }
        }

        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}