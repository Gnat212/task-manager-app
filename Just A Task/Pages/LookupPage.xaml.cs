using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для LookupPage.xaml
    /// </summary>
    public partial class LookupPage : Page
    {
        public LookupPage()
        {
            InitializeComponent();
        }

        private async void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();

            var currentListBox = sender as ListBox;
            if (currentListBox.SelectedItem != null)
            {
                var task = currentListBox.SelectedItem as Task;
                if (ActiveWindow.CurrentUser.IsAdmin)
                    await PageInit.InitAdminTaskDetailPage(task);
                else
                    await PageInit.InitUserTaskDetailPage(task);
            }
            AlertPanel.CloseLoadingCircle();
        }
    }
}
