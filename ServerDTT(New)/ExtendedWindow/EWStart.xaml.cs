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
using ServerDTT_New_.SupportClass;
using System.Threading;

namespace ServerDTT_New_.ExtendedWindow
{
    /// <summary>
    /// Interaction logic for EWStart.xaml
    /// </summary>
    public partial class EWStart : UserControl
    {
        MediaAct mediaAct = new MediaAct();
        public List<TextBlock> txtBlockStudentNameList = new List<TextBlock>();
        public List<TextBlock> txtBlockStudentPointList = new List<TextBlock>();
        Thread thread;
        User_Control.UCStart uCStart;
        double time = 0;

        public EWStart()
        {
            ImageBrush imageBrush = new ImageBrush();
            mediaAct.Upload(imageBrush, "Background.jpg");
            this.Background = imageBrush;
            InitializeComponent();
            InitEW();
        }

        public void UpdateUC(User_Control.UCStart ucStart)
        {
            uCStart = ucStart;
        }

        void InitEW()
        {
            mediaAct.Upload(videoIntro, "Start_VideoIntro.mp4");
            mediaAct.Upload(videoStudentStart, "Start_VideoStudentStart.mp4");
            mediaAct.Upload(soundFalse, "Start_SoundFalse.wav");
            mediaAct.Upload(soundStart, "Start_SoundStart.mp3");
            mediaAct.Upload(soundTrue, "Start_SoundTrue.wav");
            mediaAct.Upload(soundFinish, "Start_SoundFinish.mp3");
            mediaAct.Upload(soundIntro, "Start_SoundIntro.mp3");

            for (int i = 0; i < 4; i++)
            {
                TextBlock txtBlockStudentName = new TextBlock { Text = "Thi Sinh" + (i + 1).ToString(), FontWeight = FontWeights.DemiBold, FontFamily = new FontFamily("Open Sans"), Foreground = Brushes.White, FontSize = 28, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom };
                //Viewbox viewboxStudentName = new Viewbox();
                //viewboxStudentName.Child = txtBlockStudentName;
                //viewboxStudentName.SetValue(Grid.ColumnProperty, i + 1);
                txtBlockStudentName.SetValue(Grid.ColumnProperty, i + 1);
                txtBlockStudentName.SetValue(Grid.RowProperty, 0);
                txtBlockStudentName.SetValue(Grid.RowSpanProperty, 2);
                gridStudentsName.Children.Add(txtBlockStudentName);
                txtBlockStudentNameList.Add(txtBlockStudentName);

                TextBlock txtBlockStudentPoint = new TextBlock { Text = "0", FontWeight = FontWeights.DemiBold, FontFamily = new FontFamily("Open Sans"), FontSize = 25, Foreground = Brushes.White, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Top };
                txtBlockStudentPoint.SetValue(Grid.ColumnProperty, i + 1);
                txtBlockStudentPoint.SetValue(Grid.RowProperty, 1);
                txtBlockStudentPoint.SetValue(Grid.RowSpanProperty, 2);
                txtBlockStudentPointList.Add(txtBlockStudentPoint);
                gridStudentsName.Children.Add(txtBlockStudentPoint);
            }

            HideAll();
        }

        void ThreadStart()
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            int x = (end.Hour * 3600 + end.Minute * 60 + end.Second) - (start.Second + start.Minute * 60 + start.Hour * 3600);
            while (x< 4)
            {
                end = DateTime.Now;
                x = (end.Hour * 3600 + end.Minute * 60 + end.Second) - (start.Second + start.Minute * 60 + start.Hour * 3600);
            }
            this.Dispatcher.Invoke(() =>
            {
                gridStudentsName.Visibility = Visibility.Visible;
                txtBlockPoint.Visibility = Visibility.Visible;
                txtBlockStudent.Visibility = Visibility.Visible;
                txtBlockQuestion.Visibility = Visibility.Visible;
                uCStart.ShowNextQuestion(0);
            });
        }

        public void HideAll()
        {
            mediaAct.Stop(soundFalse);
            mediaAct.Stop(soundFinish);
            mediaAct.Stop(soundIntro);
            mediaAct.Stop(soundStart);
            mediaAct.Stop(soundTrue);

            mediaAct.Stop(videoIntro);
            mediaAct.Stop(videoStudentStart);

            HideGridIntro();
            HideGridStudentContest();
        }

        public void HideGridIntro()
        {
            videoIntro.Visibility = Visibility.Hidden;
            gridIntro.Visibility = Visibility.Hidden;
        }

        public void HideGridStudentContest()
        {
            videoStudentStart.Visibility = Visibility.Hidden;
            txtBlockPoint.Visibility = Visibility.Hidden;
            txtBlockQuestion.Visibility = Visibility.Hidden;
            gridStudentsName.Visibility = Visibility.Hidden;
            imgQuestion.Visibility = Visibility.Hidden;
            mediaQuestion.Visibility = Visibility.Hidden;
            mediaAct.Stop(videoStudentStart);
            mediaAct.Stop(mediaQuestion);
            gridStudentContest.Visibility = Visibility.Hidden;
            time = 3;
            if (thread != null)
                thread.Abort();
        }

        private void SoundStart_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(videoStudentStart);
            time = 2.3;
            thread = new Thread(ThreadStart);
            thread.Start();
        }

        private void VideoStudentStart_MediaEnded(object sender, RoutedEventArgs e)
        {
            time = 0;
            if (thread != null)
                thread.Abort();
        }

        private void VideoIntro_MediaEnded(object sender, RoutedEventArgs e)
        {
            videoIntro.Visibility = Visibility.Hidden;
        }
    }
}
