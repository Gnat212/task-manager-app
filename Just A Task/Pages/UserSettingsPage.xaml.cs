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
    /// Логика взаимодействия для UserSettingsPage.xaml
    /// </summary>
    public partial class UserSettingsPage : Page
    {
        public UserSettingsPage() => InitializeComponent();

        public User currentUser = null;

        private void CloseButton_Click(object sender, RoutedEventArgs e) => ActiveWindow.Get().UserSettingsFrame.Visibility = Visibility.Collapsed;

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
                result.Password = PasswordTextBox.Text;
                result.Email = EmailTextBox.Text.Trim();
                result.TelegramUserId = TgUidTextBox.Text.Trim();
                if (result.IsAdmin)
                {
                    dbContext.SmtpServer.First().SmtpServerIp = SmtpIpTextBlock.Text.Trim();
                    dbContext.SmtpServer.First().SmtpServerPort = SmtpPortTextBlock.Text.Trim();
                    dbContext.SmtpServer.First().SmtpLogin = SmtpLoginTextBlock.Text.Trim();
                    dbContext.SmtpServer.First().SmtpPassword = SmtpPasswordTextBlock.Text;
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


                ActiveWindow.CurrentUser = result;


                PageInit.InitAllPages();
                ActiveWindow.Get().UserPhoto.ImageSource = Extensions.ByteArrayToImageSource(ActiveWindow.CurrentUser.PhotoOrDefault);

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

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var dbContext = TaskDbEntities.NewContext;
            var window = ActiveWindow.Get();
            var users = dbContext.User.ToList();
            users.Sort(new UserComparer());
            window._loginPage.UsersComboBox.ItemsSource = users;
            window.UserSettingsFrame.Visibility = Visibility.Collapsed;

            Extensions.ClearLogin();

            window.LoginFrame.Navigate(window._loginPage);
            window.LoginFrame.Visibility = Visibility.Visible;
            window._loginPage.Visibility = Visibility.Visible;
            ActiveWindow.PauseRefreshDbTimer();
        }
    }
}
