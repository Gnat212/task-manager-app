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
    /// Логика взаимодействия для AdminTaskDetailCancelledPage.xaml
    /// </summary>
    public partial class AdminTaskDetailCancelledPage : Page
    {
        public AdminTaskDetailCancelledPage() => InitializeComponent();

        public Task currentTask = null;

        private void ClearForm()
        {
            HeaderTextBox.Text = string.Empty;
            MessageTextBox.Text = string.Empty;
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
            ActiveWindow.Get().TaskDetailFrame.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();
            var dbContext = TaskDbEntities.NewContext;
            var tempTasks = dbContext.Task.ToList();
            var dbTask = tempTasks.FirstOrDefault(x => x.Id.Equals(currentTask.Id));
            var currentUser = dbContext.User.SingleOrDefault(x => x.Id.Equals(ActiveWindow.CurrentUser.Id));
            if (currentUser != null)
            {
                if (dbTask != null)
                {
                    if (dbTask.CurrentStatus == currentTask.CurrentStatus)
                    {
                        dbContext.Task.Remove(dbTask);
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

        private async void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();
            var dbContext = TaskDbEntities.NewContext;
            var tempTasks = dbContext.Task.ToList();
            var dbTask = tempTasks.FirstOrDefault(x => x.Id.Equals(currentTask.Id));
            var currentUser = dbContext.User.SingleOrDefault(x => x.Id.Equals(ActiveWindow.CurrentUser.Id));
            if (currentUser != null)
            {
                if (dbTask != null)
                {
                    if (dbTask.CurrentStatus == currentTask.CurrentStatus)
                    {
                        dbTask.IsCancelled = false;
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
                            Notifications.SendNotifications(dbTask, NotificationType.TaskStatusUpdate);
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
    }
}
