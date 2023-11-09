using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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
    /// Логика взаимодействия для UserTaskDetailInWorkAndFiredPage.xaml
    /// </summary>
    public partial class UserTaskDetailInWorkAndFiredPage : Page
    {
        public UserTaskDetailInWorkAndFiredPage() => InitializeComponent();

        public Task currentTask = null;
        public string _filePath = string.Empty;
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

        private void ReportTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReportTextBox.Text))
                ToCheckButton.IsEnabled = false;
            else
                ToCheckButton.IsEnabled = true;
        }

        private async void ToCheckButton_Click(object sender, RoutedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();
            var dbContext = TaskDbEntities.NewContext;
            int id = currentTask.Id;
            var result = dbContext.Task.SingleOrDefault(x => x.Id.Equals(id));
            var currentUser = dbContext.User.SingleOrDefault(x => x.Id.Equals(ActiveWindow.CurrentUser.Id));
            if (currentUser != null)
            {
                if (result != null)
                {
                    if (result.CurrentStatus == currentTask.CurrentStatus)
                    {
                        try
                        {
                            if (_filePath != string.Empty)
                            {
                                var taskFile = dbContext.TaskFile.SingleOrDefault(x => x.TaskId.Equals(result.Id));
                                if (taskFile != null) 
                                {
                                    taskFile.AttachedFile = File.ReadAllBytes(_filePath);
                                    taskFile.AttachedFileName = System.IO.Path.GetFileName(_filePath);
                                }
                                else
                                {
                                    var newTaskFile = new TaskFile()
                                    {
                                        TaskId = result.Id,
                                        AttachedFile = File.ReadAllBytes(_filePath),
                                        AttachedFileName = System.IO.Path.GetFileName(_filePath)
                                    };
                                    dbContext.TaskFile.Add(newTaskFile);
                                }
                                
                            }
                                
                        }
                        catch (FileNotFoundException)
                        {
                            HandyControl.Controls.MessageBox.Show("Выбранный файл не найден", "Ошибка", MessageBoxButton.OK);
                            return;
                        }
                        result.Report = ReportTextBox.Text;
                        result.IsReadyToCheck = true;

                        try
                        {
                            dbContext.Database.Connection.Open();
                            dbContext.Database.Connection.Close();

                            dbContext.SaveChanges();
                        }
                        catch (SqlException)
                        {
                            HandyControl.Controls.MessageBox.Show("Нет подключения к базе данных", "Ошибка", MessageBoxButton.OK);
                            Application.Current.Shutdown();
                        }

                        await System.Threading.Tasks.Task.Run(() =>
                        {
                            Notifications.SendNotifications(result, NotificationType.TaskStatusUpdate);
                        }
                        );

                        OnPageClosing();
                        AlertPanel.CallCompletePanel();
                    }
                    else
                    {
                        OnPageClosing();
                        AlertPanel.CallTaskStatusChangedPanel();
                    }
                }
                else
                {
                    OnPageClosing();
                    AlertPanel.CallTaskRemovedPanel();
                }
            }
            else
            {
                OnPageClosing();
                var window = ActiveWindow.Get();
                var users = dbContext.User.ToList();
                users.Sort(new UserComparer());
                window._loginPage.UsersComboBox.ItemsSource = users;

                window.LoginFrame.Visibility = Visibility.Visible;
                window._loginPage.Visibility = Visibility.Visible;

                AlertPanel.CallCurrentUserRemovedPanel();
            }
            AlertPanel.CloseLoadingCircle();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => OnPageClosing();

        private void AttachArchive_Click(object sender, RoutedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = openFileDialog.SafeFileName;

                FileNameTextBlock.Text = fileName;
                _filePath = filePath;
            }
            AlertPanel.CloseLoadingCircle();
        }

        private void AdminReportButton_Click(object sender, RoutedEventArgs e) => RejectPanel.Visibility = Visibility.Visible;
        private void CloseAdminReportButton_Click(object sender, RoutedEventArgs e) => RejectPanel.Visibility = Visibility.Collapsed;

    }
}
