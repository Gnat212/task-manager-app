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

namespace TaskManagerRemakeApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddTaskPage.xaml
    /// </summary>
    public partial class AddTaskPage : Page
    {
        public AddTaskPage()
        {
            InitializeComponent();
        }

        private void CheckFormFill()
        {
            if (UsersCheckComboBox.SelectedItems.Count > 0 && CanConfirmUserComboBox.SelectedItem != null && !string.IsNullOrWhiteSpace(HeaderTextBox.Text) && GetTimeDatePicker.SelectedDate != null && EndTimeDatePicker.SelectedDate != null)
                AddButton.IsEnabled = true;
            else
                AddButton.IsEnabled = false;
        }
        private void ClearForm()
        {
            HeaderTextBox.Clear();
            MessageTextBox.Clear();
            CanConfirmUserComboBox.SelectedIndex = -1;
            CanConfirmUserComboBox.ItemsSource = null;
            UsersCheckComboBox.UnselectAll();
            UsersCheckComboBox.ItemsSource = null;
            GetTimeDatePicker.SelectedDate = null;
            EndTimeDatePicker.SelectedDate = null;
        }

        private void SetUsersToCheckComboBox()
        {
            var dbContext = TaskDbEntities.NewContext;
            var users = dbContext.User.Where(x => !x.IsAdmin).ToList();
            users.Sort(new UserComparer());
            UsersCheckComboBox.ItemsSource = users;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetUsersToCheckComboBox();
            AddButton.IsEnabled = false;
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

        private void ClearButton_Click(object sender, RoutedEventArgs e) => ClearForm();

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();
            var dbContext = TaskDbEntities.NewContext;
            int addedTaskId = 0;

            var task = new Task()
            {
                Header = HeaderTextBox.Text,
                Message = MessageTextBox.Text,
                GetTime = GetTimeDatePicker.SelectedDate ?? DateTime.MinValue,
                EndTime = EndTimeDatePicker.SelectedDate ?? DateTime.MaxValue,
                Report = string.Empty,
                IsComplete = false,
                IsReadyToCheck = false,
            };
            dbContext.Task.Add(task);
            try
            {
                dbContext.Database.Connection.Open();
                dbContext.Database.Connection.Close();

                dbContext.SaveChanges();
                addedTaskId = dbContext.Task.ToList().Last().Id;
            }
            catch (SqlException)
            {
                HandyControl.Controls.MessageBox.Show("Нет подключения к базе данных", "Ошибка", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }

           

            for (int i = 0; i < UsersCheckComboBox.SelectedItems.Count; i++)
            {
                var userId = (UsersCheckComboBox.SelectedItems[i] as User).Id;
                var canConfirmUserId = (CanConfirmUserComboBox.SelectedItem as User).Id;
                var taskUser = new TaskUser()
                {
                    TaskId = addedTaskId,
                    UserId = userId,
                    IsReadOnly = userId == canConfirmUserId ? false : true,
                };
                dbContext.TaskUser.Add(taskUser);
            }
            dbContext.SaveChanges();

            await System.Threading.Tasks.Task.Run(() =>
            {
                Notifications.SendNotifications(dbContext.Task.ToList().LastOrDefault(x => x.Id.Equals(addedTaskId)), NotificationType.TaskAdd);
            }
            );

            ClearForm();
            PageInit.InitAllPages();
            SetUsersToCheckComboBox();
            AlertPanel.CallCompletePanel();
            AlertPanel.CloseLoadingCircle();
        }
    }
}
