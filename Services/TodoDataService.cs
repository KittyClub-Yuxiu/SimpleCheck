using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SimpleCheck.Models;

namespace SimpleCheck.Services
{
    public class TodoDataService
    {
        private readonly string _dataFilePath;

        public TodoDataService()
        {
            // 获取应用程序数据文件夹路径
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appDataFolder, "SimpleCheck");
            
            // 确保应用程序文件夹存在
            Directory.CreateDirectory(appFolder);
            
            // 设置数据文件路径
            _dataFilePath = Path.Combine(appFolder, "todo.json");
        }

        public List<TodoItem> LoadTodos()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    string jsonContent = File.ReadAllText(_dataFilePath);
                    return JsonSerializer.Deserialize<List<TodoItem>>(jsonContent) ?? [];
                }
            }
            catch (Exception ex)
            {
                // 处理异常，例如日志记录
                Console.WriteLine($"加载待办事项失败: {ex.Message}");
            }

            return [];
        }

        public bool SaveTodos(List<TodoItem> todos)
        {
            try
            {
                string jsonContent = JsonSerializer.Serialize(todos, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_dataFilePath, jsonContent);
                return true;
            }
            catch (Exception ex)
            {
                // 处理异常，例如日志记录
                Console.WriteLine($"保存待办事项失败: {ex.Message}");
                return false;
            }
        }
    }
}