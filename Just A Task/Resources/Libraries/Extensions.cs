using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Globalization;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Data.SqlClient;
using TaskManagerRemakeApp.Data;
using System.Windows.Threading;
using System.Net.Mail;
using System.Net;

namespace TaskManagerRemakeApp.Resources.Libraries
{
    public static class Extensions
    {
        public static bool CheckConnection()
        {
            var dbContext = TaskDbEntities.NewContext;
            try
            {
                dbContext.Database.Connection.Open();
                dbContext.Database.Connection.Close();
            }
            catch (SqlException)
            {
                return false;
            }
            return true;
        }

        private static Action EmptyDelegate = delegate () { };
        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        public static ImageSource ByteArrayToImageSource(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }

        public static byte[] ImageSourceToByteArray(Uri uri)
        {
            var image = new BitmapImage(uri);
            byte[] data;
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        public static void SaveLogin(int userId) 
        {
            string path = "logindata.dat";
            using (BinaryWriter bw =  new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate)))
            {
                bw.Write(userId);
            }
        }

        public static void ClearLogin()
        {
            string path = "logindata.dat";
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                bw.Write(-1);
            }
        }
    }
}
