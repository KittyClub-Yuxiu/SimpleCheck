using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SimpleCheck
{
    public partial class TaskEditorWindow : Window
    {
        public TaskEditorWindow()
    {
        try 
        {
            InitializeComponent(); // 这里是绝大多数闪退的发生地
            
            // 只有初始化成功才绑定事件
            this.MouseDown += TaskEditorWindow_MouseDown;
        }
        catch (Exception ex)
        {
            // 这一步非常重要！它会弹窗告诉你具体的错误原因
            // 如果是 XAML 错误，它会显示 InnerException
            string message = $"窗口初始化失败:\n{ex.Message}";
            if (ex.InnerException != null)
            {
                message += $"\n\n内部错误(通常是根源):\n{ex.InnerException.Message}";
            }
            MessageBox.Show(message, "严重错误 - 截图给我");
            
            // 既然初始化失败，就不要让程序继续运行这个窗口了
            this.Close(); 
        }
    }
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            // 只有当点击的是窗口本身（拖动区域）时才拖动，防止影响输入控件
            // 这里的判断比较简单，通常不需要改，除非拖动导致了卡顿
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        
        private void TaskEditorWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 安全转换：先尝试获取点击源
            if (e.OriginalSource is DependencyObject clickedElement)
            {
                // 关键修复：如果点击的是 Run (文字的一部分)，它不是 Visual，需要找它的父级
                if (!(clickedElement is Visual || clickedElement is System.Windows.Media.Media3D.Visual3D))
                {
                    if (clickedElement is FrameworkContentElement fce)
                    {
                        clickedElement = fce.Parent;
                    }
                }

                // 再次检查，确保是 Visual 才能进行后续的 VisualTreeHelper 操作
                if (clickedElement is Visual || clickedElement is System.Windows.Media.Media3D.Visual3D)
                {
                    if (!IsElementInTimePicker(clickedElement))
                    {
                        CloseAllTimePickers();
                    }
                }
            }
        }
        
        private bool IsElementInTimePicker(DependencyObject element)
        {
            while (element != null && element != this)
            {
                if (element is Popup popup && popup.Name == "PART_Popup")
                {
                    return true;
                }

                // 再次防护：确保 element 是 Visual 才能 GetParent
                if (element is Visual || element is System.Windows.Media.Media3D.Visual3D)
                {
                    element = VisualTreeHelper.GetParent(element);
                }
                else
                {
                    // 如果遇到了非 Visual 元素（理论上上面处理过，这里是双重保险），尝试通过逻辑父级向上找
                    if (element is FrameworkContentElement fce)
                        element = fce.Parent;
                    else
                        break; // 无法继续向上查找
                }
            }
            return false;
        }
        
        private void CloseAllTimePickers()
        {
            // 优化：不要每次点击都遍历整个可视化树，这非常消耗性能！
            // 只有当确实有 DatePicker 打开时才可能需要关闭。
            // 但由于无法直接知道哪个打开了，我们至少确保查找代码是安全的。
            
            // 注意：如果窗口内容非常多，FindVisualChildren 可能会导致短暂卡顿
            foreach (DatePicker datePicker in FindVisualChildren<DatePicker>(this))
            {
                if (datePicker.IsDropDownOpen)
                {
                    datePicker.IsDropDownOpen = false;
                }
            }
        }
        
        // 你的 FindVisualChildren 方法本身逻辑是标准的，保留即可
        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                // 同样加入 Visual 检查
                if (depObj is Visual || depObj is System.Windows.Media.Media3D.Visual3D)
                {
                    int count = 0;
                    try { count = VisualTreeHelper.GetChildrenCount(depObj); } catch { }

                    for (int i = 0; i < count; i++)
                    {
                        DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                        if (child != null && child is T)
                        {
                            yield return (T)child;
                        }

                        foreach (T childOfChild in FindVisualChildren<T>(child))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }
    }
}