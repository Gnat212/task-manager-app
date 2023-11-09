using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaskManagerRemakeApp.Data;

namespace TaskManagerRemakeApp.Resources.Libraries
{
    public static class ActiveWindow
    {
        public static User CurrentUser { get; set; }

        public static MainWindow Get()
        {
            return Application.Current.MainWindow as MainWindow;
        }

        public static void PauseRefreshDbTimer() => Get()._timer.Stop();
        public static void StartRefreshDbTimer() => Get()._timer.Start();
    }
}
