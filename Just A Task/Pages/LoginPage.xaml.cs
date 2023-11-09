using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private TaskDbEntities dbContext = TaskDbEntities.NewContext;

        public EventHandler LoginCompleted;
        public LoginPage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UsersComboBox.ItemsSource = dbContext.User.ToList();
        }

        private void OnUsersComboboxTextChanged(object sender, TextChangedEventArgs e)
        {
            UsersComboBox.IsDropDownOpen = true;
            var tb = (TextBox)e.OriginalSource;
            tb.Select(tb.SelectionStart + tb.SelectionLength, 0);
            CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(UsersComboBox.ItemsSource);
            cv.Filter = s =>
                s.ToString().IndexOf(UsersComboBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();
            if (UsersComboBox.SelectedItem != null)
            {
                if (PasswordTextBox.Text == (UsersComboBox.SelectedItem as User).Password)
                {
                    LoginCompleted?.Invoke(this, EventArgs.Empty);
                    PasswordTextBox.Clear();
                    UsersComboBox.SelectedItem = null;
                    UsersComboBox.ItemsSource = null;
                    UsersComboBox.IsDropDownOpen = false;
                    WrongPasswordTextBox.Visibility = Visibility.Collapsed;
                    Visibility = Visibility.Collapsed;
                    ActiveWindow.Get().LoginFrame.Visibility = Visibility.Collapsed;
                }
                else
                {
                    WrongPasswordTextBox.Visibility = Visibility.Visible;
                }
            }
            else
            {
                UsersComboBox.Focus();
            }
            AlertPanel.CloseLoadingCircle();
        }
    }
}
