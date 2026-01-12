using System;
using System.IO;
using System.Text.Json;

namespace SimpleCheck.Services
{
    public class AppSettings
    {
        public bool CloseToTray { get; set; } = true; // 默认最小化到托盘
    }

    public class SettingsService
    {
        private readonly string _filePath;
        public AppSettings Settings { get; private set; }

        public SettingsService()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(folder, "SimpleCheck");
            Directory.CreateDirectory(appFolder);
            _filePath = Path.Combine(appFolder, "settings.json");
            
            LoadSettings();
        }

        public void LoadSettings()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    Settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                else
                {
                    Settings = new AppSettings();
                }
            }
            catch
            {
                Settings = new AppSettings();
            }
        }

        public void SaveSettings()
        {
            try
            {
                var json = JsonSerializer.Serialize(Settings);
                File.WriteAllText(_filePath, json);
            }
            catch { }
        }
    }
}