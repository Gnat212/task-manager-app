using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerRemakeApp.Data;
using TaskManagerRemakeApp;
using TaskManagerRemakeApp.Pages;
using Xceed.Wpf.Toolkit;
using HandyControl;
using Task = TaskManagerRemakeApp.Data.Task;
using System.Windows;
using System.Data.Entity;
using System.Windows.Controls;
using HandyControl.Controls;
using System.Collections.ObjectModel;
using HandyControl.Tools.Extension;
using Xceed.Wpf.AvalonDock.Layout;
using System.IO;
using System.Windows.Media.Imaging;

namespace TaskManagerRemakeApp.Resources.Libraries
{
    public static class PageInit
    {
        private static readonly MainWindow window = ActiveWindow.Get();
        
        public static void InitAllPages()
        {
            var dbContext = TaskDbEntities.NewContext;

            void InitLookupPage()
            {

                if (ActiveWindow.CurrentUser.IsAdmin)
                {
                    var adminTasks = dbContext.Task.ToList();

                    window.lookupPage.InWorkListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == Status.InWork).ToList();
                    window.lookupPage.InWorkCountTextBox.Text = "Заданий: " + window.lookupPage.InWorkListBox.Items.Count;

                    window.lookupPage.OnCheckListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == Status.OnCheck).ToList();
                    window.lookupPage.OnCheckCountTextBox.Text = "Заданий: " + window.lookupPage.OnCheckListBox.Items.Count;

                    window.lookupPage.FiredListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == Status.Fired).ToList();
                    window.lookupPage.FiredCountTextBox.Text = "Заданий: " + window.lookupPage.FiredListBox.Items.Count;

                    window.lookupPage.CompletedListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == Status.Completed).ToList();
                    window.lookupPage.CompletedCountTextBox.Text = "Заданий: " + window.lookupPage.CompletedListBox.Items.Count;

                    window.lookupPage.RejectedListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == Status.Rejected).ToList();
                    window.lookupPage.RejectedCountTextBox.Text = "Заданий: " + window.lookupPage.RejectedListBox.Items.Count;

                    window.lookupPage.CancelledListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == Status.Cancelled).ToList();
                    window.lookupPage.CancelledCountTextBox.Text = "Заданий: " + window.lookupPage.CancelledListBox.Items.Count;
                }

                else
                {
                    var userTaskUser = dbContext.TaskUser.Where(x => x.UserId == ActiveWindow.CurrentUser.Id).ToList();
                    var userTasks = new List<Task>();
                    foreach (var taskUser in userTaskUser)
                        userTasks.Add(taskUser.Task);

                    window.lookupPage.InWorkListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == Status.InWork).ToList();
                    window.lookupPage.InWorkCountTextBox.Text = "Заданий: " + window.lookupPage.InWorkListBox.Items.Count;

                    window.lookupPage.OnCheckListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == Status.OnCheck).ToList();
                    window.lookupPage.OnCheckCountTextBox.Text = "Заданий: " + window.lookupPage.OnCheckListBox.Items.Count;

                    window.lookupPage.FiredListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == Status.Fired).ToList();
                    window.lookupPage.FiredCountTextBox.Text = "Заданий: " + window.lookupPage.FiredListBox.Items.Count;

                    window.lookupPage.CompletedListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == Status.Completed).ToList();
                    window.lookupPage.CompletedCountTextBox.Text = "Заданий: " + window.lookupPage.CompletedListBox.Items.Count;

                    window.lookupPage.RejectedListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == Status.Rejected).ToList();
                    window.lookupPage.RejectedCountTextBox.Text = "Заданий: " + window.lookupPage.RejectedListBox.Items.Count;

                    window.lookupPage.CancelledListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == Status.Cancelled).ToList();
                    window.lookupPage.CancelledCountTextBox.Text = "Заданий: " + window.lookupPage.CancelledListBox.Items.Count;
                }
            }
            void InitInWorkPage()
            {
                var page = window.inWorkPage;
                var neededStatus = Status.InWork;

                if (ActiveWindow.CurrentUser.IsAdmin)
                {
                    var adminTasks = dbContext.Task.ToList();

                    page.MainListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
                else
                {
                    var userTaskUser = dbContext.TaskUser.Where(x => x.UserId == ActiveWindow.CurrentUser.Id).ToList();
                    var userTasks = new List<Task>();
                    foreach (var taskUser in userTaskUser)
                        userTasks.Add(taskUser.Task);

                    page.MainListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
            }
            void InitOnCheckPage()
            {
                var page = window.onCheckPage;
                var neededStatus = Status.OnCheck;

                if (ActiveWindow.CurrentUser.IsAdmin)
                {
                    var adminTasks = dbContext.Task.ToList();

                    page.MainListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
                else
                {
                    var userTaskUser = dbContext.TaskUser.Where(x => x.UserId == ActiveWindow.CurrentUser.Id).ToList();
                    var userTasks = new List<Task>();
                    foreach (var taskUser in userTaskUser)
                        userTasks.Add(taskUser.Task);

                    page.MainListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
            }
            void InitFiredPage()
            {
                var page = window.firedPage;
                var neededStatus = Status.Fired;

                if (ActiveWindow.CurrentUser.IsAdmin)
                {
                    var adminTasks = dbContext.Task.ToList();

                    page.MainListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
                else
                {
                    var userTaskUser = dbContext.TaskUser.Where(x => x.UserId == ActiveWindow.CurrentUser.Id).ToList();
                    var userTasks = new List<Task>();
                    foreach (var taskUser in userTaskUser)
                        userTasks.Add(taskUser.Task);

                    page.MainListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
            }
            void InitCompletedPage()
            {
                var page = window.completedPage;
                var neededStatus = Status.Completed;

                if (ActiveWindow.CurrentUser.IsAdmin)
                {
                    var adminTasks = dbContext.Task.ToList();

                    page.MainListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
                else
                {
                    var userTaskUser = dbContext.TaskUser.Where(x => x.UserId == ActiveWindow.CurrentUser.Id).ToList();
                    var userTasks = new List<Task>();
                    foreach (var taskUser in userTaskUser)
                        userTasks.Add(taskUser.Task);

                    page.MainListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
            }
            void InitRejectedPage()
            {
                var page = window.rejectedPage;
                var neededStatus = Status.Rejected;

                if (ActiveWindow.CurrentUser.IsAdmin)
                {
                    var adminTasks = dbContext.Task.ToList();

                    page.MainListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
                else
                {
                    var userTaskUser = dbContext.TaskUser.Where(x => x.UserId == ActiveWindow.CurrentUser.Id).ToList();
                    var userTasks = new List<Task>();
                    foreach (var taskUser in userTaskUser)
                        userTasks.Add(taskUser.Task);

                    page.MainListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
            }
            void InitCancelledPage()
            {
                var page = window.cancelledPage;
                var neededStatus = Status.Cancelled;

                if (ActiveWindow.CurrentUser.IsAdmin)
                {
                    var adminTasks = dbContext.Task.ToList();

                    page.MainListBox.ItemsSource = adminTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
                else
                {
                    var userTaskUser = dbContext.TaskUser.Where(x => x.UserId == ActiveWindow.CurrentUser.Id).ToList();
                    var userTasks = new List<Task>();
                    foreach (var taskUser in userTaskUser)
                        userTasks.Add(taskUser.Task);

                    page.MainListBox.ItemsSource = userTasks.Where(x => x.CurrentStatus == neededStatus).ToList();
                    page.CountTextBlock.Text = "Заданий: " + page.MainListBox.Items.Count;
                }
            }
            void InitUsersPage()
            {
                var page = window.usersPage;

                var users = dbContext.User.ToList();
                users.Sort(new UserComparer());

                page.UsersListBox.ItemsSource = users;
                page.CountTextBlock.Text = "Всего пользовтелей: " + users.Count;
            }

            var currentUser = dbContext.User.SingleOrDefault(x => x.Id.Equals(ActiveWindow.CurrentUser.Id));
            if (currentUser != null)
            {
                InitLookupPage();
                InitInWorkPage();
                InitOnCheckPage();
                InitFiredPage();
                InitCompletedPage();
                InitRejectedPage();
                InitCancelledPage();
                InitUsersPage();
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

            GC.Collect();
        }
        
        public static void UncheckAllListBoxes()
        {
            window.lookupPage.CancelledListBox.SelectedItem = null;
            window.lookupPage.CompletedListBox.SelectedItem = null;
            window.lookupPage.FiredListBox.SelectedItem = null;
            window.lookupPage.InWorkListBox.SelectedItem = null;
            window.lookupPage.OnCheckListBox.SelectedItem = null;
            window.lookupPage.RejectedListBox.SelectedItem = null;

            window.cancelledPage.MainListBox.SelectedItem = null;
            window.completedPage.MainListBox.SelectedItem = null;
            window.firedPage.MainListBox.SelectedItem = null;
            window.inWorkPage.MainListBox.SelectedItem = null;
            window.onCheckPage.MainListBox.SelectedItem = null;
            window.rejectedPage.MainListBox.SelectedItem = null;

            window.usersPage.UsersListBox.SelectedItem = null;
        }
        

        public static async System.Threading.Tasks.Task InitAdminTaskDetailPage(Task task)
        {
            var dbContext = TaskDbEntities.NewContext;
            Task currentTask = null;
            User currentUser = null;
            await System.Threading.Tasks.Task.Run(() =>
            {
                currentTask = dbContext.Task.FirstOrDefault(x => x.Id.Equals(task.Id));
                currentUser = dbContext.User.FirstOrDefault(x => x.Id.Equals(ActiveWindow.CurrentUser.Id));
            });

            if (currentUser != null)
            {
                if (currentTask != null)
                {
                    if (task.CurrentStatus == currentTask.CurrentStatus)
                    {
                        window.TaskDetailFrame.Visibility = Visibility.Visible;


                        switch (task.CurrentStatus)
                        {
                            case Status.InWork:
                            case Status.Fired:
                                ActiveWindow.PauseRefreshDbTimer();
                                var users = new ObservableCollection<User>(task.UsersList);
                                var checkedUsers = new ObservableCollection<User>(users);
                                var usersIds = new ObservableCollection<int>();
                                foreach (var user in users)
                                    usersIds.Add(user.Id);
                                var allUsersExceptChecked = dbContext.User.Where(x => !x.IsAdmin && !usersIds.Contains(x.Id)).ToList();
                                var page1 = window.adminTaskDetailInWorkAndFiredPage;
                                page1.HeaderTextBox.Text = task.Header;
                                page1.MessageTextBox.Text = task.Message;
                                page1.AdminReportButton.Visibility = task.IsReturned ? Visibility.Visible : Visibility.Collapsed;
                                page1.AdminReportTextBox.Text = task.AdminReport ?? string.Empty;
                                page1.UsersCheckComboBox.ItemsSource = users;
                                page1.UsersCheckComboBox.SelectAll();
                                users.AddRange(allUsersExceptChecked);
                                page1.CanConfirmUserComboBox.ItemsSource = checkedUsers;
                                page1.CanConfirmUserComboBox.SelectedItem = checkedUsers.First(x => x.Id == task.Responsible.Id);
                                page1.GetTimeDatePicker.SelectedDate = task.GetTime;
                                page1.EndTimeDatePicker.SelectedDate = task.EndTime;
                                page1.currentTask = task;
                                window.TaskDetailFrame.Navigate(window.adminTaskDetailInWorkAndFiredPage);
                                break;

                            case Status.Completed:
                                ActiveWindow.PauseRefreshDbTimer();
                                var page2 = window.adminTaskDetailCompletedAndRejectedPage;
                                page2.HeaderTextBox.Text = task.Header;
                                page2.MessageTextBox.Text = task.Message;
                                page2.AdminReportButton.Visibility = task.IsReturned ? Visibility.Visible : Visibility.Collapsed;
                                page2.AdminReportTextBox.Text = task.AdminReport ?? string.Empty;
                                page2.AdminReportButton.Content = "Была в доработке";
                                page2.RejectReasonTextBlock.Text = "Причина доработки";
                                page2.ReportTextBox.Text = task.Report;
                                page2.UsersListBox.ItemsSource = task.UsersList;
                                page2.CanConfirmUserListBox.ItemsSource = task.UsersList.Where(x => x.Id == task.Responsible.Id);
                                page2.GetTimeDatePicker.SelectedDate = task.GetTime;
                                page2.EndTimeDatePicker.SelectedDate = task.EndTime;
                                page2.currentTask = task;
                                page2.AttachedFilePanel.Visibility = TaskDbEntities.NewContext.TaskFile.FirstOrDefault(x => x.TaskId == task.Id) == null ? Visibility.Collapsed : Visibility.Visible;
                                window.TaskDetailFrame.Navigate(window.adminTaskDetailCompletedAndRejectedPage);
                                break;

                            case Status.Rejected:
                                ActiveWindow.PauseRefreshDbTimer();
                                var page21 = window.adminTaskDetailCompletedAndRejectedPage;
                                page21.HeaderTextBox.Text = task.Header;
                                page21.MessageTextBox.Text = task.Message;
                                page21.AdminReportButton.Visibility = task.AdminReport != null ? Visibility.Visible : Visibility.Collapsed;
                                page21.AdminReportTextBox.Text = task.AdminReport ?? string.Empty;
                                page21.AdminReportButton.Content = "Отклонена";
                                page21.RejectReasonTextBlock.Text = "Причина отклонения";
                                page21.ReportTextBox.Text = task.Report;
                                page21.UsersListBox.ItemsSource = task.UsersList;
                                page21.CanConfirmUserListBox.ItemsSource = task.UsersList.Where(x => x.Id == task.Responsible.Id);
                                page21.GetTimeDatePicker.SelectedDate = task.GetTime;
                                page21.EndTimeDatePicker.SelectedDate = task.EndTime;
                                page21.currentTask = task;
                                page21.AttachedFilePanel.Visibility = TaskDbEntities.NewContext.TaskFile.FirstOrDefault(x => x.TaskId == task.Id) == null ? Visibility.Collapsed : Visibility.Visible;
                                window.TaskDetailFrame.Navigate(window.adminTaskDetailCompletedAndRejectedPage);
                                break;

                            case Status.OnCheck:
                                ActiveWindow.PauseRefreshDbTimer();
                                var page3 = window.adminTaskDetailOnCheckPage;
                                page3.HeaderTextBox.Text = task.Header;
                                page3.MessageTextBox.Text = task.Message;
                                page3.AdminReportButton.Visibility = task.IsReturned ? Visibility.Visible : Visibility.Collapsed;
                                page3.AdminReportTextBox.Text = task.AdminReport ?? string.Empty;
                                page3.ReportTextBox.Text = task.Report;
                                page3.UsersListBox.ItemsSource = task.UsersList;
                                page3.CanConfirmUserListBox.ItemsSource = task.UsersList.Where(x => x.Id == task.Responsible.Id);
                                page3.GetTimeDatePicker.SelectedDate = task.GetTime;
                                page3.EndTimeDatePicker.SelectedDate = task.EndTime;
                                page3.currentTask = task;
                                page3.AttachedFilePanel.Visibility = TaskDbEntities.NewContext.TaskFile.FirstOrDefault(x => x.TaskId == task.Id) == null ? Visibility.Collapsed : Visibility.Visible;
                                window.TaskDetailFrame.Navigate(window.adminTaskDetailOnCheckPage);
                                break;

                            case Status.Cancelled:
                                ActiveWindow.PauseRefreshDbTimer();
                                var page4 = window.adminTaskDetailCancelledPage;
                                page4.HeaderTextBox.Text = task.Header;
                                page4.MessageTextBox.Text = task.Message;
                                page4.UsersListBox.ItemsSource = task.UsersList;
                                page4.CanConfirmUserListBox.ItemsSource = task.UsersList.Where(x => x.Id == task.Responsible.Id);
                                page4.GetTimeDatePicker.SelectedDate = task.GetTime;
                                page4.EndTimeDatePicker.SelectedDate = task.EndTime;
                                page4.currentTask = task;
                                window.TaskDetailFrame.Navigate(window.adminTaskDetailCancelledPage);
                                break;
                            case Status.Undefinded:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        AlertPanel.CallTaskStatusChangedPanel();
                        InitAllPages();
                    }
                }
                else
                {
                    AlertPanel.CallTaskRemovedPanel();
                    InitAllPages();
                }
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
        }

        public static async System.Threading.Tasks.Task InitUserTaskDetailPage(Task task)
        {
            var dbContext = TaskDbEntities.NewContext;
            Task currentTask = null;
            User currentUser = null;
            await System.Threading.Tasks.Task.Run(() =>
            {
                currentTask = dbContext.Task.FirstOrDefault(x => x.Id.Equals(task.Id));
                currentUser = dbContext.User.FirstOrDefault(x => x.Id.Equals(ActiveWindow.CurrentUser.Id));
            });

            if (currentUser != null)
            {
                if (currentTask != null)
                {
                    if (task.CurrentStatus == currentTask.CurrentStatus)
                    {
                        window.TaskDetailFrame.Visibility = Visibility.Visible;

                        switch (task.CurrentStatus)
                        {
                            case Status.InWork:
                            case Status.Fired:
                                ActiveWindow.PauseRefreshDbTimer();
                                var page1 = window.userTaskDetailInWorkAndFiredPage;
                                page1.HeaderTextBox.Text = task.Header;
                                page1.MessageTextBox.Text = task.Message;
                                page1.AdminReportButton.Visibility = task.IsReturned ? Visibility.Visible : Visibility.Collapsed;
                                page1.AdminReportTextBox.Text = task.AdminReport ?? string.Empty;
                                page1.ReportTextBox.Text = task.Report;
                                page1.UsersListBox.ItemsSource = task.UsersList;
                                page1.CanConfirmUserListBox.ItemsSource = task.UsersList.Where(x => x.Id == task.Responsible.Id);
                                page1.GetTimeDatePicker.SelectedDate = task.GetTime;
                                page1.EndTimeDatePicker.SelectedDate = task.EndTime;
                                page1.currentTask = task;
                                page1._filePath = string.Empty;
                                page1.FileNameTextBlock.Text = "Выберите файл";
                                if (task.Responsible.Id == ActiveWindow.CurrentUser.Id)
                                    page1.ToCheckButton.Visibility = Visibility.Visible;
                                else
                                    page1.ToCheckButton.Visibility = Visibility.Collapsed;
                                window.TaskDetailFrame.Navigate(window.userTaskDetailInWorkAndFiredPage);
                                break;

                            case Status.Completed:
                                ActiveWindow.PauseRefreshDbTimer();
                                var page2 = window.userTaskDetailCompletedAndRejectedPage;
                                page2.HeaderTextBox.Text = task.Header;
                                page2.MessageTextBox.Text = task.Message;
                                page2.AdminReportButton.Visibility = task.IsReturned ? Visibility.Visible : Visibility.Collapsed;
                                page2.AdminReportTextBox.Text = task.AdminReport ?? string.Empty;
                                page2.AdminReportButton.Content = "Была в доработке";
                                page2.RejectReasonTextBlock.Text = "Причина доработки";
                                page2.ReportTextBox.Text = task.Report;
                                page2.UsersListBox.ItemsSource = task.UsersList;
                                page2.CanConfirmUserListBox.ItemsSource = task.UsersList.Where(x => x.Id == task.Responsible.Id);
                                page2.GetTimeDatePicker.SelectedDate = task.GetTime;
                                page2.EndTimeDatePicker.SelectedDate = task.EndTime;
                                page2.currentTask = task;
                                page2.AttachedFilePanel.Visibility = TaskDbEntities.NewContext.TaskFile.FirstOrDefault(x => x.TaskId == task.Id) == null ? Visibility.Collapsed : Visibility.Visible;
                                window.TaskDetailFrame.Navigate(window.userTaskDetailCompletedAndRejectedPage);
                                break;

                            case Status.Rejected:
                                ActiveWindow.PauseRefreshDbTimer();
                                var page21 = window.userTaskDetailCompletedAndRejectedPage;
                                page21.HeaderTextBox.Text = task.Header;
                                page21.MessageTextBox.Text = task.Message;
                                page21.AdminReportButton.Visibility = task.AdminReport != null ? Visibility.Visible : Visibility.Collapsed;
                                page21.AdminReportTextBox.Text = task.AdminReport ?? string.Empty;
                                page21.AdminReportButton.Content = "Отклонена";
                                page21.RejectReasonTextBlock.Text = "Причина отклонения";
                                page21.ReportTextBox.Text = task.Report;
                                page21.UsersListBox.ItemsSource = task.UsersList;
                                page21.CanConfirmUserListBox.ItemsSource = task.UsersList.Where(x => x.Id == task.Responsible.Id);
                                page21.GetTimeDatePicker.SelectedDate = task.GetTime;
                                page21.EndTimeDatePicker.SelectedDate = task.EndTime;
                                page21.currentTask = task;
                                page21.AttachedFilePanel.Visibility = TaskDbEntities.NewContext.TaskFile.FirstOrDefault(x => x.TaskId == task.Id) == null ? Visibility.Collapsed : Visibility.Visible;
                                window.TaskDetailFrame.Navigate(window.userTaskDetailCompletedAndRejectedPage);
                                break;

                            case Status.OnCheck:
                                ActiveWindow.PauseRefreshDbTimer();
                                var page3 = window.userTaskDetailOnCheckPage;
                                page3.HeaderTextBox.Text = task.Header;
                                page3.MessageTextBox.Text = task.Message;
                                page3.AdminReportButton.Visibility = task.IsReturned ? Visibility.Visible : Visibility.Collapsed;
                                page3.AdminReportTextBox.Text = task.AdminReport ?? string.Empty;
                                page3.ReportTextBox.Text = task.Report;
                                page3.UsersListBox.ItemsSource = task.UsersList;
                                page3.CanConfirmUserListBox.ItemsSource = task.UsersList.Where(x => x.Id == task.Responsible.Id);
                                page3.GetTimeDatePicker.SelectedDate = task.GetTime;
                                page3.EndTimeDatePicker.SelectedDate = task.EndTime;
                                page3.currentTask = task;
                                page3.AttachedFilePanel.Visibility = TaskDbEntities.NewContext.TaskFile.FirstOrDefault(x => x.TaskId == task.Id) == null ? Visibility.Collapsed : Visibility.Visible;
                                window.TaskDetailFrame.Navigate(window.userTaskDetailOnCheckPage);
                                break;

                            case Status.Cancelled:
                                ActiveWindow.PauseRefreshDbTimer();
                                var page4 = window.userTaskDetailCancelledPage;
                                page4.HeaderTextBox.Text = task.Header;
                                page4.MessageTextBox.Text = task.Message;
                                page4.UsersListBox.ItemsSource = task.UsersList;
                                page4.CanConfirmUserListBox.ItemsSource = task.UsersList.Where(x => x.Id == task.Responsible.Id);
                                page4.GetTimeDatePicker.SelectedDate = task.GetTime;
                                page4.EndTimeDatePicker.SelectedDate = task.EndTime;
                                page4.currentTask = task;
                                window.TaskDetailFrame.Navigate(window.userTaskDetailCancelledPage);
                                break;
                            case Status.Undefinded:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        AlertPanel.CallTaskStatusChangedPanel();
                        InitAllPages();
                    }
                }
                else
                {
                    AlertPanel.CallTaskRemovedPanel();
                    InitAllPages();
                }
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

            
        }

        public static void InitUserDetailPage(User user)
        {
            var dbContext = TaskDbEntities.NewContext;
            var result = dbContext.User.FirstOrDefault(x => x.Id.Equals(user.Id));
            var currentUser = dbContext.User.FirstOrDefault(x => x.Id.Equals(ActiveWindow.CurrentUser.Id));
            if (currentUser != null)
            {
                if (result != null)
                {
                    window.userDetailPage = new UserDetailPage();

                    window.UserDetailFrame.Visibility = Visibility.Visible;

                    string[] surnameName = user.Username.Split(' ');

                    window.userDetailPage.SurnameTextBox.Text = surnameName[0];
                    window.userDetailPage.NameTextBox.Text = surnameName[1];
                    window.userDetailPage.PasswordTextBox.Text = user.Password;
                    window.userDetailPage.EmailTextBox.Text = user.Email;
                    window.userDetailPage.TgUidTextBox.Text = user.TelegramUserId;
                    window.userDetailPage.CurrentPhotoImageBrush.ImageSource = Extensions.ByteArrayToImageSource(user.PhotoOrDefault);

                    if (user.IsAdmin)
                    {
                        if (user.Id == 1)
                        {
                            window.userDetailPage.IsAdminToggleButton.Visibility = Visibility.Collapsed;
                            window.userDetailPage.DeleteButton.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            window.userDetailPage.IsAdminToggleButton.Visibility = Visibility.Visible;
                            window.userDetailPage.DeleteButton.Visibility = Visibility.Visible;
                        }
                        window.userDetailPage.IsAdminToggleButton.IsChecked = true;
                        window.userDetailPage.IsAdminTextBlock.Text = "Администратор";
                        window.userDetailPage.CanAddTaskPanel.Visibility = Visibility.Collapsed;
                        window.userDetailPage.CanAddTaskToggleButton.IsChecked = user.CanAddTask;
                        window.userDetailPage.CanChangeUserPanel.Visibility = Visibility.Collapsed;
                        window.userDetailPage.CanChangeUserToggleButton.IsChecked = user.CanChangeUsers;
                    }
                    else
                    {
                        window.userDetailPage.IsAdminToggleButton.Visibility = Visibility.Visible;
                        window.userDetailPage.DeleteButton.Visibility = Visibility.Visible;
                        window.userDetailPage.IsAdminToggleButton.IsChecked = false;
                        window.userDetailPage.IsAdminTextBlock.Text = "Пользователь";
                        window.userDetailPage.CanAddTaskPanel.Visibility = Visibility.Visible;
                        window.userDetailPage.CanAddTaskToggleButton.IsChecked = user.CanAddTask;
                        window.userDetailPage.CanChangeUserPanel.Visibility = Visibility.Visible;
                        window.userDetailPage.CanChangeUserToggleButton.IsChecked = user.CanChangeUsers;
                    }

                    window.userDetailPage.currentUser = result;
                    
                    window.UserDetailFrame.Navigate(window.userDetailPage);
                    window.usersPage.UsersListBox.SelectedItem = null;
                }
                else
                {
                    InitAllPages();
                    window.usersPage.UsersListBox.SelectedItem = null;
                    AlertPanel.CallUserRemovedPanel();
                }
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
                window.usersPage.UsersListBox.SelectedItem = null;

                AlertPanel.CallCurrentUserRemovedPanel();
            }           
        }

        public static void InitUserSettingsPage(User user)
        {
            var dbContext = TaskDbEntities.NewContext;
            var currentUser = dbContext.User.FirstOrDefault(x => x.Id.Equals(user.Id));

            if (currentUser != null)
            {
                window.UserSettingsFrame.Visibility = Visibility.Visible;
                window.userSettingsPage.PasswordTextBox.Text = user.Password;
                window.userSettingsPage.UsernameTextBox.Text = user.Username;
                window.userSettingsPage.EmailTextBox.Text = user.Email;
                window.userSettingsPage.TgUidTextBox.Text = user.TelegramUserId;
                window.userSettingsPage.IsAdminTextBlock.Text = user.IsAdmin ? "Администратор" : "Пользователь";
                window.userSettingsPage.SmtpPanel.Visibility = user.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
                window.userSettingsPage.SmtpIpTextBlock.Text = dbContext.SmtpServer.First().SmtpServerIp;
                window.userSettingsPage.SmtpPortTextBlock.Text = dbContext.SmtpServer.First().SmtpServerPort;
                window.userSettingsPage.SmtpLoginTextBlock.Text = dbContext.SmtpServer.First().SmtpLogin;
                window.userSettingsPage.SmtpPasswordTextBlock.Text = dbContext.SmtpServer.First().SmtpPassword;
                window.userSettingsPage.PhotoImageSelector.Hide();
                window.userSettingsPage.PhotoImageSelector.Show();
                window.userSettingsPage.CurrentPhotoImageBrush.ImageSource = Extensions.ByteArrayToImageSource(user.PhotoOrDefault);

                window.userSettingsPage.currentUser = user;

                window.UserSettingsFrame.Navigate(window.userSettingsPage);
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
            
        }

        public static void InitAddUserPage()
        {
            var dbContext = TaskDbEntities.NewContext;
            var currentUser = dbContext.User.FirstOrDefault(x => x.Id.Equals(ActiveWindow.CurrentUser.Id));
            if (currentUser != null)
            {
                window.addUserPage.AddButton.IsEnabled = false;
                window.addUserPage.IsAdminToggleButton.IsChecked = false;
                window.addUserPage.CurrentPhotoImageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/defaultUserIcon.png"));

                window.AddUserFrame.Visibility = Visibility.Visible;
                window.AddUserFrame.Navigate(window.addUserPage);
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
            
        }
    }
}
