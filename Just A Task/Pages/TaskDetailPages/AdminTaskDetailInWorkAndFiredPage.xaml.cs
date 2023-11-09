using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Логика взаимодействия для AdminTaskDetailInWorkAndFiredPage.xaml
    /// </summary>
    public partial class AdminTaskDetailInWorkAndFiredPage : Page
    {
        public AdminTaskDetailInWorkAndFiredPage() => InitializeComponent();

        public Task currentTask = null;
        
        private void ClearForm()
        {
            HeaderTextBox.Text = string.Empty;
            MessageTextBox.Text = string.Empty;
            UsersCheckComboBox.SelectedIndex = -1; UsersCheckComboBox.ItemsSource = null;
            CanConfirmUserComboBox.SelectedIndex = -1; CanConfirmUserComboBox.ItemsSource = null;
            GetTimeDatePicker.SelectedDate = null;
            EndTimeDatePicker.SelectedDate = null;
        }
        
        private void CheckFormFill()
        {
            if (UsersCheckComboBox.SelectedItems.Count > 0 && CanConfirmUserComboBox.SelectedItem != null && !string.IsNullOrWhiteSpace(HeaderTextBox.Text) && GetTimeDatePicker.SelectedDate != null && EndTimeDatePicker.SelectedDate != null)
                SaveButton.IsEnabled = true;
            else
                SaveButton.IsEnabled = false;
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

        private void UsersCheckComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CanConfirmUserComboBox.ItemsSource = UsersCheckComboBox.SelectedItems;
            CheckFormFill();
        }

        private void HeaderTextBox_TextChanged(object sender, TextChangedEventArgs e) => CheckFormFill();

        private void CanConfirmUserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => CheckFormFill();

        private void GetTimeDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => CheckFormFill();

        private void EndTimeDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => CheckFormFill();

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
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
                        result.Header = HeaderTextBox.Text;
                        result.Message = MessageTextBox.Text;
                        result.GetTime = GetTimeDatePicker.SelectedDate ?? DateTime.MinValue;
                        result.EndTime = EndTimeDatePicker.SelectedDate ?? DateTime.MaxValue;

                        dbContext.TaskUser.RemoveRange(dbContext.TaskUser.Where(x => x.TaskId.Equals(result.Id)));
                        for (int i = 0; i < UsersCheckComboBox.SelectedItems.Count; i++)
                        {
                            var userId = (UsersCheckComboBox.SelectedItems[i] as User).Id;
                            var canConfirmUserId = (CanConfirmUserComboBox.SelectedItem as User).Id;
                            var taskUser = new TaskUser()
                            {
                                TaskId = result.Id,
                                UserId = userId,
                                IsReadOnly = userId == canConfirmUserId ? false : true,
                            };
                            dbContext.TaskUser.Add(taskUser);
                        }

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
                            Notifications.SendNotifications(result, NotificationType.TaskInfoUpdate);
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

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
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
                    if (result.CurrentStatus == Status.InWork)
                    {
                        result.IsCancelled = true;

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

        private void AdminReportButton_Click(object sender, RoutedEventArgs e) => RejectPanel.Visibility = Visibility.Visible;
        private void CloseAdminReportButton_Click(object sender, RoutedEventArgs e) => RejectPanel.Visibility = Visibility.Collapsed;

    }
}
