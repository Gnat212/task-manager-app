using HandyControl.Themes;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TaskManagerRemakeApp.Data;
using TaskManagerRemakeApp.Pages;
using TaskManagerRemakeApp.Pages.MenuPages;
using TaskManagerRemakeApp.Pages.TaskDetailPages;
using TaskManagerRemakeApp.Resources.Libraries;

namespace TaskManagerRemakeApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly LoginPage _loginPage = new LoginPage();

        public readonly AddTaskPage addTaskPage = new AddTaskPage();
        public readonly AddUserPage addUserPage = new AddUserPage();
        public readonly UsersPage usersPage = new UsersPage();

        public readonly UserSettingsPage userSettingsPage = new UserSettingsPage();
        public UserDetailPage userDetailPage = null;

        public readonly DispatcherTimer _timer = new DispatcherTimer() ;

        public readonly LookupPage lookupPage = new LookupPage();
        public readonly InWorkPage inWorkPage = new InWorkPage();
        public readonly OnCheckPage onCheckPage = new OnCheckPage();
        public readonly FiredPage firedPage = new FiredPage();
        public readonly RejectedPage rejectedPage = new RejectedPage();
        public readonly CompletedPage completedPage = new CompletedPage();
        public readonly CancelledPage cancelledPage = new CancelledPage();
        
        public readonly NullPage nullPage = new NullPage();

        public AdminTaskDetailCancelledPage adminTaskDetailCancelledPage = null;
        public AdminTaskDetailCompletedAndRejectedPage adminTaskDetailCompletedAndRejectedPage = null;
        public AdminTaskDetailInWorkAndFiredPage adminTaskDetailInWorkAndFiredPage = null;
        public AdminTaskDetailOnCheckPage adminTaskDetailOnCheckPage = null;

        public UserTaskDetailCancelledPage userTaskDetailCancelledPage = null;
        public UserTaskDetailCompletedAndRejectedPage userTaskDetailCompletedAndRejectedPage = null;
        public UserTaskDetailInWorkAndFiredPage userTaskDetailInWorkAndFiredPage = null;
        public UserTaskDetailOnCheckPage userTaskDetailOnCheckPage = null;

        private void InitAdminCondition()
        {
            if (userTaskDetailInWorkAndFiredPage != null)
            {
                userTaskDetailCancelledPage = null;
                userTaskDetailCompletedAndRejectedPage = null;
                userTaskDetailInWorkAndFiredPage = null;
                userTaskDetailOnCheckPage = null;
            }

            AddTaskButton.Visibility = Visibility.Visible;
            ShowUsersButton.Visibility = Visibility.Visible;
            adminTaskDetailCancelledPage = new AdminTaskDetailCancelledPage();
            adminTaskDetailCompletedAndRejectedPage = new AdminTaskDetailCompletedAndRejectedPage();
            adminTaskDetailInWorkAndFiredPage = new AdminTaskDetailInWorkAndFiredPage();
            adminTaskDetailOnCheckPage = new AdminTaskDetailOnCheckPage();
        }
        private void InitUserCondition()
        {
            if (adminTaskDetailInWorkAndFiredPage != null)
            {
                adminTaskDetailCancelledPage = null;
                adminTaskDetailCompletedAndRejectedPage = null;
                adminTaskDetailInWorkAndFiredPage = null;
                adminTaskDetailOnCheckPage = null;
            }
            
            AddTaskButton.Visibility = ActiveWindow.CurrentUser.CanAddTask ? Visibility.Visible : Visibility.Collapsed;
            ShowUsersButton.Visibility = ActiveWindow.CurrentUser.CanChangeUsers ? Visibility.Visible : Visibility.Collapsed;
            userTaskDetailCancelledPage = new UserTaskDetailCancelledPage();
            userTaskDetailCompletedAndRejectedPage = new UserTaskDetailCompletedAndRejectedPage();
            userTaskDetailInWorkAndFiredPage = new UserTaskDetailInWorkAndFiredPage();
            userTaskDetailOnCheckPage = new UserTaskDetailOnCheckPage();
        }
        
        public MainWindow()
        {
            InitializeComponent();
            ConfigHelper.Instance.SetLang("ru");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _loginPage.LoginCompleted += LoginCompleted;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Extensions.CheckConnection())
            {
                int savedUserId = -1;
                using (BinaryReader reader = new BinaryReader(File.Open("logindata.dat", FileMode.OpenOrCreate)))
                {
                    if (reader.PeekChar() > -1)
                        try
                        {
                            savedUserId = reader.ReadInt32();
                        }
                        catch { }
                }

                if (savedUserId != -1)
                {
                    var dbContext = TaskDbEntities.NewContext;
                    var user = dbContext.User.FirstOrDefault(x => x.Id == savedUserId);
                    if (user != null)
                    {
                        ActiveWindow.CurrentUser = user;
                        UserPhoto.ImageSource = Extensions.ByteArrayToImageSource(ActiveWindow.CurrentUser.PhotoOrDefault);
                        UserNameTextBlock.Text = ActiveWindow.CurrentUser.Username;

                        if (ActiveWindow.CurrentUser.IsAdmin)
                            InitAdminCondition();
                        else
                            InitUserCondition();


                        PageInit.InitAllPages();
                        ActiveWindow.StartRefreshDbTimer();
                        ShowTasksButton.IsChecked = true;
                        MainFrame.Navigate(lookupPage);
                    }
                    else
                    {
                        LoginFrame.Navigate(_loginPage);
                    }
                }
                else
                {
                    LoginFrame.Navigate(_loginPage);
                }

                
                _timer.Interval = TimeSpan.FromSeconds(15);
                _timer.Tick += RefreshDbTimerTick;
            }
            else
            {
                HandyControl.Controls.MessageBox.Show("Нет подключения к базе данных", "Ошибка", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }
            
        }

        private void LoginCompleted(object sender, EventArgs e)
        {
            ActiveWindow.CurrentUser = _loginPage.UsersComboBox.SelectedItem as User;
            UserPhoto.ImageSource = Extensions.ByteArrayToImageSource(ActiveWindow.CurrentUser.PhotoOrDefault);
            UserNameTextBlock.Text = ActiveWindow.CurrentUser.Username;

            if (ActiveWindow.CurrentUser.IsAdmin)
                InitAdminCondition();
            else
                InitUserCondition();

            
            PageInit.InitAllPages();
            ActiveWindow.StartRefreshDbTimer();
            ShowTasksButton.IsChecked = true;
            Extensions.SaveLogin((_loginPage.UsersComboBox.SelectedItem as User).Id);
            MainFrame.Navigate(lookupPage);
        }

        private void UncheckShowTasksButton() => ShowTasksButton.IsChecked = false;
        
        private void UncheckAllDetailTaskButtons()
        {
            var buttons = new List<ToggleButton>() { InWorkButton, OnCheckButton, FiredButton, CompletedButton, RejectedButton, CancelledButton };
            foreach (var button in buttons)
                button.IsChecked = false;
        }
        private void UncheckAllDetailTaskButtonsExcept(ToggleButton button)
        {
            var buttons = new List<ToggleButton>() { InWorkButton, OnCheckButton, FiredButton, CompletedButton, RejectedButton, CancelledButton };
            buttons.Remove(button);
            foreach (var _button in buttons)
                _button.IsChecked = false;
        }
        
        private void ShowTasksButton_Checked(object sender, RoutedEventArgs e)
        {
            DetailTaskPanel.Visibility = Visibility.Visible;
            MainFrame.Navigate(lookupPage);
        }

        private void ShowTasksButton_Unchecked(object sender, RoutedEventArgs e)
        {
            UncheckAllDetailTaskButtons();
            DetailTaskPanel.Visibility = Visibility.Collapsed;
        }

        private void DetailTaskButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            UncheckAllDetailTaskButtonsExcept(button);
            switch (button.Name)
            {
                case "InWorkButton":
                    MainFrame.Navigate(inWorkPage);
                    break;
                case "OnCheckButton":
                    MainFrame.Navigate(onCheckPage);
                    break;
                case "FiredButton":
                    MainFrame.Navigate(firedPage);
                    break;
                case "CompletedButton":
                    MainFrame.Navigate(completedPage);
                    break;
                case "RejectedButton":
                    MainFrame.Navigate(rejectedPage);
                    break;
                case "CancelledButton":
                    MainFrame.Navigate(cancelledPage);
                    break;
                default:
                    break;
            }
        }

        private void DetailTaskButton_Unchecked(object sender, RoutedEventArgs e)
        {
            bool isAnyoneButtonChecked = false;
            var buttons = new List<ToggleButton>() { InWorkButton, OnCheckButton, FiredButton, CompletedButton, RejectedButton, CancelledButton };
            foreach (var button in buttons)
            {
                if (button.IsChecked ?? true)
                {
                    isAnyoneButtonChecked = true;
                    break;
                }
            }
            if (isAnyoneButtonChecked)
                return;
            else
                MainFrame.Navigate(lookupPage);
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckShowTasksButton();
            MainFrame.Navigate(addTaskPage);
        }

        private void ShowUsersButton_Click(object sender, RoutedEventArgs e)
        {
            UncheckShowTasksButton();
            MainFrame.Navigate(usersPage);
        }

        private void RefreshDbTimerTick(object sender, EventArgs e) => PageInit.InitAllPages();
        private void NotificationPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var notificationPanel = sender as Border;
            if (notificationPanel.Visibility == Visibility.Visible)
            {
                var startAnim = new DoubleAnimation();
                startAnim.From = 0;
                startAnim.To = 0.99;
                startAnim.Duration = TimeSpan.FromMilliseconds(500);
                Storyboard.SetTarget(startAnim, notificationPanel);
                Storyboard.SetTargetProperty(startAnim, new PropertyPath(OpacityProperty));

                var endAnim = new DoubleAnimation();
                endAnim.From = 1;
                endAnim.To = 0;
                endAnim.BeginTime = TimeSpan.FromMilliseconds(2500);
                endAnim.Duration = TimeSpan.FromMilliseconds(2000);
                Storyboard.SetTarget(endAnim, notificationPanel);
                Storyboard.SetTargetProperty(endAnim, new PropertyPath(OpacityProperty));

                var storyboard = new Storyboard { Children = new TimelineCollection { startAnim, endAnim } };
                storyboard.Completed += (s, ex) =>
                {
                    notificationPanel.Visibility = Visibility.Collapsed;
                };
                storyboard.Begin();
            }
        }

        private void ProfileSettingsButton_Click(object sender, RoutedEventArgs e) => PageInit.InitUserSettingsPage(ActiveWindow.CurrentUser);
        
    }
}
