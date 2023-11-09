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
using TaskManagerRemakeApp.Resources.Libraries;

namespace TaskManagerRemakeApp.Pages.MenuPages
{
    /// <summary>
    /// Логика взаимодействия для CancelledPage.xaml
    /// </summary>
    public partial class CancelledPage : Page
    {
        public CancelledPage()
        {
            InitializeComponent();
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AlertPanel.CallLoadingCircle();
            var currentListBox = sender as ListBox;
            if (currentListBox.SelectedItem != null)
            {
                var task = currentListBox.SelectedItem as Data.Task;
                if (ActiveWindow.CurrentUser.IsAdmin)
                    PageInit.InitAdminTaskDetailPage(task);
                else
                    PageInit.InitUserTaskDetailPage(task);
            }
            AlertPanel.CloseLoadingCircle();
        }
        
    }
}
