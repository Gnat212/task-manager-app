using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskManagerRemakeApp.Data;
using TaskManagerRemakeApp.Resources.Libraries;
using Task = TaskManagerRemakeApp.Data.Task;

namespace TaskManagerRemakeApp.Pages.TaskDetailPages
{
    /// <summary>
    /// Логика взаимодействия для UserTaskDetailCompletedAndRejectedPage.xaml
    /// </summary>
    public partial class UserTaskDetailCompletedAndRejectedPage : Page
    {
        public UserTaskDetailCompletedAndRejectedPage() => InitializeComponent();

        public Task currentTask = null;
        private void ClearForm()
        {
            HeaderTextBox.Text = string.Empty;
            MessageTextBox.Text = string.Empty;
            ReportTextBox.Text = string.Empty;
            UsersListBox.ItemsSource = null;
            CanConfirmUserListBox.ItemsSource = null;
            GetTimeDatePicker.SelectedDate = null;
            EndTimeDatePicker.SelectedDate = null;
        }
        private void OnPageClosing()
        {
            var page = ActiveWindow.Get();
            PageInit.InitAllPages();
            PageInit.UncheckAllListBoxes();
            ClearForm();
            ActiveWindow.StartRefreshDbTimer();
            page.TaskDetailFrame.Navigate(page.nullPage);
            RejectPanel.Visibility = Visibility.Collapsed;
            ActiveWindow.Get().TaskDetailFrame.Visibility = Visibility.Collapsed;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => OnPageClosing();

        private void DownloadArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            var dbContext = TaskDbEntities.NewContext;

            var taskFile = dbContext.TaskFile.FirstOrDefault(x => x.TaskId.Equals(currentTask.Id));
            if (taskFile != null)
            {
                saveFileDialog.FileName = taskFile.AttachedFileName;
                saveFileDialog.DefaultExt = $".{taskFile.AttachedFileExt}";
                saveFileDialog.Filter = $"{taskFile.AttachedFileExt.ToUpper()} файл (*.{taskFile.AttachedFileExt})|*.{taskFile.AttachedFileExt}";
            }

            if (saveFileDialog.ShowDialog() == true)
            {
                if (taskFile != null)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, taskFile.AttachedFile);
                }
            }
            AlertPanel.CloseLoadingCircle();
        }

        private void AdminReportButton_Click(object sender, RoutedEventArgs e) => RejectPanel.Visibility = Visibility.Visible;
        private void CloseAdminReportButton_Click(object sender, RoutedEventArgs e) => RejectPanel.Visibility = Visibility.Collapsed;

    }
}
