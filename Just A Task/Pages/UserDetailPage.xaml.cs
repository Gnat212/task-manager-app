using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace TaskManagerRemakeApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserDetailPage.xaml
    /// </summary>
    public partial class UserDetailPage : Page
    {
        public UserDetailPage() => InitializeComponent();

        public Data.User currentUser = null;

        private void CheckFormFill()
        {
            if (!string.IsNullOrWhiteSpace(NameTextBox.Text) && !string.IsNullOrWhiteSpace(SurnameTextBox.Text) && !string.IsNullOrWhiteSpace(PasswordTextBox.Text))
                SaveButton.IsEnabled = true;
            else
                SaveButton.IsEnabled = false;
        }

        private void OnPageClosing()
        {
            var window = ActiveWindow.Get();
            PageInit.InitAllPages();
            PageInit.UncheckAllListBoxes();
            ActiveWindow.StartRefreshDbTimer();
            window.UserDetailFrame.Navigate(window.nullPage);
            window.userDetailPage = null;
            window.UserDetailFrame.Visibility = Visibility.Collapsed;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => ActiveWindow.Get().UserDetailFrame.Visibility = Visibility.Collapsed;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();
            var dbContext = TaskDbEntities.NewContext;
            int id = currentUser.Id;
            var result = dbContext.User.SingleOrDefault(x => x.Id.Equals(id));
            if (result != null)
            {
                if (PhotoImageSelector.Uri != null)
                {
                    try
                    {
                        result.Photo = Extensions.ImageSourceToByteArray(PhotoImageSelector.Uri);
                    }
                    catch (FileNotFoundException)
                    {
                        HandyControl.Controls.MessageBox.Show("Выбранное изображение удалено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                result.Username = SurnameTextBox.Text.Trim() + " " + NameTextBox.Text.Trim();
                result.Password = PasswordTextBox.Text;
                result.IsAdmin = IsAdminToggleButton.IsChecked.Value;
                result.Email = EmailTextBox.Text.Trim();
                result.TelegramUserId = TgUidTextBox.Text.Trim();
                result.CanAddTask = IsAdminToggleButton.IsChecked.Value ? true : CanAddTaskToggleButton.IsChecked.Value;
                result.CanChangeUsers = IsAdminToggleButton.IsChecked.Value ? true : CanChangeUserToggleButton.IsChecked.Value;
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

                if (ActiveWindow.CurrentUser.Id == id)
                {
                    var window = ActiveWindow.Get();
                    if (result.Photo != null)
                    {
                        window.UserPhoto.ImageSource = Extensions.ByteArrayToImageSource(result.Photo);
                    }
                    window.UserNameTextBlock.Text = result.Username;

                    dbContext = TaskDbEntities.NewContext;
                    var users = dbContext.User.ToList();
                    users.Sort(new UserComparer());
                    window._loginPage.UsersComboBox.ItemsSource = users;

                    window.LoginFrame.Visibility = Visibility.Visible;
                    window._loginPage.Visibility = Visibility.Visible;
                    OnPageClosing();
                    ActiveWindow.PauseRefreshDbTimer();
                    AlertPanel.CallCompletePanel();
                    return;
                }

                OnPageClosing();
                AlertPanel.CallCompletePanel();
            }
            else
            {
                OnPageClosing();
                AlertPanel.CallUserRemovedPanel();
            }
            AlertPanel.CloseLoadingCircle();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();
            var dbContext = TaskDbEntities.NewContext;
            int id = currentUser.Id;
            var result = dbContext.User.SingleOrDefault(x => x.Id.Equals(id));
            if (result != null)
            {
                var tasksToDelete = dbContext.Task.ToList().Where(x => x.Responsible.Id == id).ToList();
                dbContext.Task.RemoveRange(tasksToDelete);
                dbContext.User.Remove(result);
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
                AlertPanel.CallUserRemovedPanel();
            }
            AlertPanel.CloseLoadingCircle();
        }

        private void CheckFormFill_Event(object sender, TextChangedEventArgs e) => CheckFormFill();



        private void IsAdminToggleButton_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (IsAdminToggleButton.IsChecked.Value)
            {
                IsAdminTextBlock.Text = "Администратор";

                CanChangeUserToggleButton.IsChecked = false;
                CanChangeUserPanel.Visibility = Visibility.Collapsed;
                CanAddTaskToggleButton.IsChecked = false;
                CanAddTaskPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                IsAdminTextBlock.Text = "Пользователь";
                CanAddTaskPanel.Visibility = Visibility.Visible;
                CanChangeUserPanel.Visibility = Visibility.Visible;
            }
        }
    }
}
