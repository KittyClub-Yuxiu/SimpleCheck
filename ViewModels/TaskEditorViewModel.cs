using System;
using System.Windows.Input;
using SimpleCheck.Models;
using SimpleCheck.Utilities;

namespace SimpleCheck.ViewModels
{
    public class TaskEditorViewModel : ViewModelBase
    {
        private TodoItem _task;
        private bool _isOneTimeTask = true;
        private bool _isDailyTask = false;
        
        public TodoItem Task
        {
            get => _task;
            set => SetProperty(ref _task, value);
        }
        
        public bool IsOneTimeTask
        {
            get => _isOneTimeTask;
            set 
            {
                SetProperty(ref _isOneTimeTask, value);
                if (value)
                {
                    _isDailyTask = false;
                    OnPropertyChanged(nameof(IsDailyTask));
                    Task.Type = TaskType.OneTime;
                }
            }
        }
        
        public bool IsDailyTask
        {
            get => _isDailyTask;
            set 
            {
                SetProperty(ref _isDailyTask, value);
                if (value)
                {
                    _isOneTimeTask = false;
                    OnPropertyChanged(nameof(IsOneTimeTask));
                    Task.Type = TaskType.Daily;
                }
            }
        }
        
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        
        public event Action<TodoItem> TaskSaved;
        public event Action TaskCancelled;
        
        public TaskEditorViewModel()
        {
            Task = new TodoItem { Content = "", LastUpdatedTime = DateTime.Now };
            SaveCommand = new RelayCommand(SaveTask);
            CancelCommand = new RelayCommand(CancelEdit);
        }
        
        private void SaveTask()
        {
            if (!string.IsNullOrWhiteSpace(Task.Content))
            {
                TaskSaved?.Invoke(Task);
            }
        }
        
        private void CancelEdit()
        {
            TaskCancelled?.Invoke();
        }
    }
}