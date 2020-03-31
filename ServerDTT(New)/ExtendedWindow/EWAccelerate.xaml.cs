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
    /// Interaction logic for EWAccelerate.xaml
    /// </summary>
    public partial class EWAccelerate : UserControl
    {
        public List<TextBlock> txtBlockStudentNameList = new List<TextBlock>();
        public List<TextBlock> txtBlockStudentTimeList = new List<TextBlock>();
        public List<TextBlock> txtBlockStudentAnswerList = new List<TextBlock>();
        MediaAct mediaAct = new MediaAct();
        Thread thread;
        public Server server;
        double time = 0;
        public EWAccelerate()
        {
            ImageBrush imageBrush = new ImageBrush();
            mediaAct.Upload(imageBrush, "Background.jpg");
            this.Background = imageBrush;
            InitializeComponent();
            InitEW();
        }

        void InitEW()
        {
            for (int i = 0; i < 4; i++)
            {
                Viewbox viewboxStudentName = new Viewbox();
                TextBlock txtBlockStudentName = new TextBlock { FontFamily = new FontFamily("Open Sans"), FontWeight=FontWeights.DemiBold, Foreground = Brushes.Black};
                viewboxStudentName.SetValue(Grid.RowProperty, 3 * i + 1);
                viewboxStudentName.SetValue(Grid.ColumnProperty, 1);
                viewboxStudentName.Child = txtBlockStudentName;
                txtBlockStudentNameList.Add(txtBlockStudentName);
                gridStudentAnswer.Children.Add(viewboxStudentName);

                Viewbox viewboxStudentAnswer = new Viewbox();
                TextBlock txtBlockStudentAnswer = new TextBlock {FontFamily = new FontFamily("Open Sans"), FontWeight=FontWeights.DemiBold, Foreground = Brushes.White};
                viewboxStudentAnswer.SetValue(Grid.RowProperty, 3 * i + 2);
                viewboxStudentAnswer.SetValue(Grid.ColumnProperty, 1);
                viewboxStudentAnswer.SetValue(Grid.ColumnSpanProperty, 2);
                viewboxStudentAnswer.Child = txtBlockStudentAnswer;
                txtBlockStudentAnswerList.Add(txtBlockStudentAnswer);
                gridStudentAnswer.Children.Add(viewboxStudentAnswer);

                Viewbox viewboxStudentTime = new Viewbox();
                TextBlock txtBlockStudentTime = new TextBlock { FontSize = 40, FontFamily = new FontFamily("Open Sans"), FontWeight=FontWeights.DemiBold, Foreground = Brushes.Black, TextAlignment = TextAlignment.Center };
                viewboxStudentTime.SetValue(Grid.RowProperty, 3 * i + 2);
                viewboxStudentTime.SetValue(Grid.ColumnProperty, 3);
                txtBlockStudentTimeList.Add(txtBlockStudentTime);
                viewboxStudentTime.Child = txtBlockStudentTime;
                gridStudentAnswer.Children.Add(viewboxStudentTime);
            }

            mediaAct.Upload(soundTrue, "Accelerate_SoundTrue.mp3");
            mediaAct.Upload(soundFalse, "Accelerate_SoundFalse.wav");
            mediaAct.Upload(soundIntro, "Accelerate_SoundIntro.mp3");

            mediaAct.Upload(videoAnswer, "Accelerate_VideoAnswer.mp4");
            mediaAct.Upload(videoIntro, "Accelerate_VideoIntro.mp4");
            mediaAct.Upload(videoQuestionStart, "Accelerate_VideoQuestionStart.mp4");
            mediaAct.Upload(videoTime, "Accelerate_VideoTime.mp4");
        }

        void HideGridStudentAnswer()
        {
            for (int i = 0; i < 4; i++)
            {
                txtBlockStudentAnswerList[i].Visibility = Visibility.Hidden;
                txtBlockStudentNameList[i].Visibility = Visibility.Hidden;
                txtBlockStudentTimeList[i].Visibility = Visibility.Hidden;
            }
            gridStudentAnswer.Visibility = Visibility.Hidden;
        }

        void HideGridQuestion()
        {
            gridQuestion.Visibility = Visibility.Hidden;
            imgQuestion.Visibility = Visibility.Hidden;
            txtBlockQuestion.Visibility = Visibility.Hidden;
        }

        void HideGridIntro()
        {
            gridIntro.Visibility = Visibility.Hidden;
        }

        public void HideAll()
        {
            mediaAct.Stop(videoAnswer);
            mediaAct.Stop(videoIntro);
            mediaAct.Stop(videoQuestion);
            mediaAct.Stop(videoQuestionStart);
            mediaAct.Stop(videoTime);

            mediaAct.Stop(soundFalse);
            mediaAct.Stop(soundIntro);
            mediaAct.Stop(soundTrue);

            HideGridQuestion();
            HideGridStudentAnswer();
            HideGridIntro();
        }

        void ThreadAnswer()
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            int x = (end.Hour * 3600000 + end.Minute * 60000 + end.Second * 1000 + end.Millisecond) - (start.Millisecond + start.Second * 1000 + start.Minute * 60000 + start.Hour * 3600000);
            while (x <= time*1000)
            {
                if (x >= 5000)
                    this.Dispatcher.Invoke(() =>
                    {
                        txtBlockStudentAnswerList[((x - 1000) / 1000) % 4].Visibility = Visibility.Visible;
                        txtBlockStudentNameList[((x - 1000) / 1000) % 4].Visibility = Visibility.Visible;
                        txtBlockStudentTimeList[((x - 1000) / 1000) % 4].Visibility = Visibility.Visible;
                    });
                end = DateTime.Now;
                x = (end.Hour * 3600000 + end.Minute * 60000 + end.Second * 1000 + end.Millisecond) - (start.Millisecond + start.Second * 1000 + start.Minute * 60000 + start.Hour * 3600000);
            }

            for (int i = 0; i < 4; i++)
                this.Dispatcher.Invoke(() =>
                {
                    txtBlockStudentAnswerList[i].Visibility = Visibility.Visible;
                    txtBlockStudentNameList[i].Visibility = Visibility.Visible;
                    txtBlockStudentTimeList[i].Visibility = Visibility.Visible;
                });
            thread.Abort();
        }

        public void StartThreadAnswer()
        {
            time = 8;
            thread = new Thread(ThreadAnswer);
            thread.Start();
        }

        private void VideoAnswer_MediaEnded(object sender, RoutedEventArgs e)
        {
            //thread.Abort();
        }

        private void VideoQuestionStart_MediaEnded(object sender, RoutedEventArgs e)
        {
            txtBlockQuestion.Visibility = Visibility.Visible;
            imgQuestion.Visibility = Visibility.Visible;
            videoQuestion.Visibility = Visibility.Visible;
        }

        private void VideoIntro_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaAct.Stop(videoIntro);
        }
    }
}
