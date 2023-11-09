using HandyControl.Controls;
using HandyControl.Tools.Extension;
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
    /// Логика взаимодействия для AddUserPage.xaml
    /// </summary>
    public partial class AddUserPage : Page
    {
        public AddUserPage() => InitializeComponent();

        private void CheckFormFill()
        {
            if (!string.IsNullOrWhiteSpace(NameTextBox.Text) && !string.IsNullOrWhiteSpace(SurnameTextBox.Text) && !string.IsNullOrWhiteSpace(PasswordTextBox.Text))
                AddButton.IsEnabled = true;
            else
                AddButton.IsEnabled = false;
        }

        private void ClearForm()
        {
            NameTextBox.Clear();
            SurnameTextBox.Clear();
            PasswordTextBox.Clear();
            IsAdminToggleButton.IsChecked = false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();
            var dbContext = TaskDbEntities.NewContext;
            var currentUser = dbContext.User.SingleOrDefault(x => x.Id.Equals(ActiveWindow.CurrentUser.Id));
            if (currentUser != null)
            {
                var newUser = new User();
                
                if (PhotoImageSelector.Uri != null)
                {
                    try
                    {
                        newUser.Photo = Extensions.ImageSourceToByteArray(PhotoImageSelector.Uri);
                    }
                    catch (FileNotFoundException)
                    {
                        HandyControl.Controls.MessageBox.Show("Выбранное изображение удалено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                newUser.Username = SurnameTextBox.Text.Trim() + " " + NameTextBox.Text.Trim();
                newUser.Password = PasswordTextBox.Text;
                newUser.IsAdmin = IsAdminToggleButton.IsChecked.Value;
                newUser.Email = EmailTextBox.Text.Trim();
                newUser.TelegramUserId = TgUidTextBox.Text.Trim();
                newUser.CanAddTask = IsAdminToggleButton.IsChecked.Value ? true : CanAddTaskToggleButton.IsChecked.Value;
                newUser.CanChangeUsers = IsAdminToggleButton.IsChecked.Value ? true : CanChangeUserToggleButton.IsChecked.Value;

                dbContext.User.Add(newUser);

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

                ClearForm();

                PageInit.InitAllPages();
                AlertPanel.CallCompletePanel();
            }
            else
            {
                var window = ActiveWindow.Get();
                var users = dbContext.User.ToList();
                users.Sort(new UserComparer());
                window._loginPage.UsersComboBox.ItemsSource = users;

                window.LoginFrame.Visibility = Visibility.Visible;
                window._loginPage.Visibility = Visibility.Visible;
                ActiveWindow.PauseRefreshDbTimer();

                AlertPanel.CallCurrentUserRemovedPanel();
            }
            AlertPanel.CloseLoadingCircle();
        }

        private void SurnameTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private void NameTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void CheckFormFill_Event(object sender, TextChangedEventArgs e) => CheckFormFill();

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var window = ActiveWindow.Get();
            ClearForm();
            window.AddUserFrame.Visibility = Visibility.Collapsed;
            window.AddUserFrame.Navigate(window.nullPage);
            
        }

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
