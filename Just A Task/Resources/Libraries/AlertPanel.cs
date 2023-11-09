using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TaskManagerRemakeApp.Resources.Libraries
{
    public static class AlertPanel
    {
        public static void CallCompletePanel()
        {
            var window = ActiveWindow.Get();
            window.CompleteNotificationPanel.Visibility = Visibility.Collapsed;
            window.CompleteNotificationPanel.Visibility = Visibility.Visible;
        }

        public static void CallTaskStatusChangedPanel()
        {
            var window = ActiveWindow.Get();
            window.TaskStatusChangedNotificationPanel.Visibility = Visibility.Collapsed;
            window.TaskStatusChangedNotificationPanel.Visibility = Visibility.Visible;
        }

        public static void CallTaskRemovedPanel()
        {
            var window = ActiveWindow.Get();
            window.TaskRemovedNotificationPanel.Visibility = Visibility.Collapsed;
            window.TaskRemovedNotificationPanel.Visibility = Visibility.Visible;
        }

        public static void CallUserRemovedPanel()
        {
            var window = ActiveWindow.Get();
            window.UserRemovedNotificationPanel.Visibility = Visibility.Collapsed;
            window.UserRemovedNotificationPanel.Visibility = Visibility.Visible;
        }

        public static void CallCurrentUserRemovedPanel()
        {
            var window = ActiveWindow.Get();
            window.CurrentUserRemovedNotificationPanel.Visibility = Visibility.Collapsed;
            window.CurrentUserRemovedNotificationPanel.Visibility = Visibility.Visible;
        }

        public static void CallLoadingCircle()
        {
            var window = ActiveWindow.Get();
            window.LoadingCircle.Visibility = Visibility.Visible;
        }

        public static void CloseLoadingCircle()
        {
            var window = ActiveWindow.Get();
            window.LoadingCircle.Visibility = Visibility.Collapsed;
        }
    }
}
