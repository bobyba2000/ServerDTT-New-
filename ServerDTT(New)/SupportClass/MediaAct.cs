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
using System.Windows.Shapes;
using System.IO;

namespace ServerDTT_New_.SupportClass
{
    class MediaAct
    {
        public void Upload(MediaElement media, string mediaName)
        {
            if (mediaName == "")
            {
                media.Source = null;
                return;
            }
            try
            {
                media.Stop();
                string Filepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                media.Source = new Uri(Filepath + "\\Resources\\" + mediaName, UriKind.Relative);
            }
            catch
            {
                media.Source = null;
                Console.Write("Loi khong load duoc");
            }
        }
        public void Play(MediaElement media)
        {
            if (media.Source == null) return;
            media.Stop();
            media.Visibility = Visibility.Visible;
            media.Play();
        }
        public void Stop(MediaElement media)
        {
            if (media.Source == null) return;
            media.Visibility = Visibility.Hidden;
            media.Stop();
        }
        public void Upload(Image image, string imageName)
        {
            try
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                string Filepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
                    + "\\Resources\\" + imageName;
                logo.UriSource = new Uri(Filepath);
                logo.EndInit();
                image.Source = logo;
            }
            catch
            {
                image.Source = null;
            }
        }
        public void Upload(ImageBrush imageBrush, string imageBrushName)
        {
            try
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                string Filepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
                    + "\\Resources\\" + imageBrushName;
                logo.UriSource = new Uri(Filepath);
                logo.EndInit();
                imageBrush.ImageSource = logo;
            }
            catch
            {
                imageBrush.ImageSource = null;
            }
        }
    }
}
