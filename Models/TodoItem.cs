using System;

namespace SimpleCheck.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Content { get; set; }
        public bool IsCompleted { get; set; }
        public TaskType Type { get; set; } // 单次 或 每日
        public DateTime LastUpdatedTime { get; set; }

        // [新增] 开始时间，为空则代表不限开始时间
        public DateTime? StartTime { get; set; }

        // [新增] 结束时间，为空则代表不限结束时间
        public DateTime? EndTime { get; set; }

        // 辅助属性：判断是否为全天任务
        public bool IsAllDay => StartTime == null && EndTime == null;
        
        // 辅助属性：用于UI显示的格式化时间字符串
        public string TimeDisplayString 
        {
            get 
            {
                if (IsAllDay) return string.Empty;
                string start = StartTime?.ToString("HH:mm") ?? "...";
                string end = EndTime?.ToString("HH:mm") ?? "...";
                return $"{start} - {end}";
            }
        }
    }
}