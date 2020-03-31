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
using System.Threading;
using ServerDTT_New_.SupportClass;

namespace ServerDTT_New_.ExtendedWindow
{
    /// <summary>
    /// Interaction logic for EWMainWindow.xaml
    /// </summary>
    public partial class EWPointSummarized : UserControl
    {
        List<Student> studentList;
        MediaAct mediaAct = new MediaAct();
        Thread thread;
        double time = 17;
        public EWPointSummarized()
        {
            ImageBrush imageBrush = new ImageBrush();
            mediaAct.Upload(imageBrush, "Background.jpg");
            this.Background = imageBrush;
            InitializeComponent();
            InitMain();
        }

        void InitMain()
        {
            mediaAct.Upload(videoPointSummarized, "VideoPointSummarized.mp4");
            mediaAct.Upload(soundFinishAll, "SoundFinishAll.mp3");
        }

        void TimeStart()
        {
            while(true)
            {
                DateTime Start = DateTime.Now;
                if (time <= 12)
                    this.Dispatcher.Invoke(() =>
                    {

                        if ((int)(time) % 3 != 0)
                        {
                            txtBlockName.Text = studentList[(int)(time) / 3].Name;
                            txtBlockPoint.Text = studentList[(int)(time) / 3].Point.ToString();
                        }
                        else
                        {
                            txtBlockName.Text = "";
                            txtBlockPoint.Text = "";
                        }
                    });
                else break;
                DateTime End = DateTime.Now;
                time += (double)(End.Ticks - Start.Ticks) / 10000000;
            }
            thread.Abort();
        }

        int SortPoint(Student a, Student b)
        {
            if (a.Point < b.Point) return -1;
            else return 1;
        }

        public void PlayVideoSummarizedPoint(List<Student> students)
        {
            soundFinishAll.Stop();
            studentList = new List<Student>(students);
            studentList.Sort(SortPoint);
            mediaAct.Play(videoPointSummarized);
            time = 0;
            thread = new Thread(TimeStart);
            thread.Start();
            txtBlockName.Visibility = Visibility.Visible;
            txtBlockPoint.Visibility = Visibility.Visible;
        }

        private void VideoPointSummarized_MediaEnded(object sender, RoutedEventArgs e)
        {
            thread.Abort();
            txtBlockName.Visibility = Visibility.Hidden;
            txtBlockPoint.Visibility = Visibility.Hidden;
            videoPointSummarized.Visibility = Visibility.Hidden;
        }

        private void VideoPointSummarized_MediaOpened(object sender, RoutedEventArgs e)
        {
            time = 0;
            txtBlockName.Visibility = Visibility.Visible;
            txtBlockPoint.Visibility = Visibility.Visible;
        }

        private void SoundFinishAll_MediaEnded(object sender, RoutedEventArgs e)
        {

        }
    }
}
