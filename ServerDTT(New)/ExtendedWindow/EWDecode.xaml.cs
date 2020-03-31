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
    /// Interaction logic for EWDecode.xaml
    /// </summary>
    public partial class EWDecode : UserControl
    {
        public List<TextBlock> txtBlockStudentNameList = new List<TextBlock>();
        public List<TextBlock> txtBlockStudentTimeList = new List<TextBlock>();
        public List<TextBlock> txtBlockStudentAnswerList = new List<TextBlock>();

        public Button[,] btnMatrixCellArray;

        MediaAct mediaAct =new MediaAct();
        double time = 0;

        const int numberOfStudent = 4;

        Thread thread;
        public EWDecode()
        {
            ImageBrush imageBrush = new ImageBrush();
            mediaAct.Upload(imageBrush, "BackgroundNoLogo.jpg");
            this.Background = imageBrush;
            InitializeComponent();

            InitEW();

            HideAll();
        }

        void InitEW()
        {
            for (int i = 0; i < numberOfStudent; i++)
            {
                Viewbox viewboxStudentName = new Viewbox();
                TextBlock txtBlockStudentName = new TextBlock { FontFamily = new FontFamily("Open Sans"), Foreground = Brushes.Black };
                viewboxStudentName.SetValue(Grid.RowProperty, 3 * i + 1);
                viewboxStudentName.SetValue(Grid.ColumnProperty, 1);
                viewboxStudentName.Child = txtBlockStudentName;
                txtBlockStudentNameList.Add(txtBlockStudentName);
                gridStudentAnswer.Children.Add(viewboxStudentName);

                Viewbox viewboxStudentAnswer = new Viewbox();
                TextBlock txtBlockStudentAnswer = new TextBlock { FontFamily = new FontFamily("Open Sans"), Foreground = Brushes.White };
                viewboxStudentAnswer.SetValue(Grid.RowProperty, 3 * i + 2);
                viewboxStudentAnswer.SetValue(Grid.ColumnProperty, 1);
                viewboxStudentAnswer.SetValue(Grid.ColumnSpanProperty, 2);
                viewboxStudentAnswer.Child = txtBlockStudentAnswer;
                txtBlockStudentAnswerList.Add(txtBlockStudentAnswer);
                gridStudentAnswer.Children.Add(viewboxStudentAnswer);

                Viewbox viewboxStudentTime = new Viewbox();
                TextBlock txtBlockStudentTime = new TextBlock { FontSize = 40, FontFamily = new FontFamily("Open Sans"), Foreground = Brushes.Black, TextAlignment = TextAlignment.Center };
                viewboxStudentTime.SetValue(Grid.RowProperty, 3 * i + 2);
                viewboxStudentTime.SetValue(Grid.ColumnProperty, 3);
                txtBlockStudentTimeList.Add(txtBlockStudentTime);
                viewboxStudentTime.Child = txtBlockStudentTime;
                gridStudentAnswer.Children.Add(viewboxStudentTime);
            }

            mediaAct.Upload(soundTrueAnswer, "Decode_SoundTrueAnswer.mp3");
            mediaAct.Upload(soundFalseAnswer, "Decode_SoundFalseAnswer.wav");
            mediaAct.Upload(soundHint, "Decode_SoundHint.mp3");
            mediaAct.Upload(soundShowMatrix, "Decode_SoundShowMatrix.mp3");
            mediaAct.Upload(soundIntro, "Decode_SoundIntro.mp3");
            mediaAct.Upload(soundTrueChoose, "Decode_SoundTrueChoose.wav");
            
            mediaAct.Upload(videoAnswer, "Decode_VideoAnswer.mp4");
            mediaAct.Upload(videoIntro, "Decode_VideoIntro.mp4");
            mediaAct.Upload(videoQuestionStart, "Decode_VideoQuestionStart.mp4");
            mediaAct.Upload(videoRule, "Decode_VideoRule.mp4");
        }

        void HideGridIntro()
        {
            gridIntro.Visibility = Visibility.Hidden;
        }

        void HideGridMatrix()
        {
            gridMatrix.Visibility = Visibility.Hidden;
        }

        void HideGridFinalAnswer()
        {
            gridFinalAnswer.Visibility = Visibility.Hidden;
        }

        void HideGridQuestion()
        {
            gridQuestion.Visibility = Visibility.Hidden;
            txtBlockQuestion1.Visibility = Visibility.Hidden;
            txtBlockQuestion2.Visibility = Visibility.Hidden;
            imgQuestion.Visibility = Visibility.Hidden;
        }

        void HideGridStudentAnswer()
        {
            for (int i = 0; i < numberOfStudent; i++)
            {
                txtBlockStudentAnswerList[i].Visibility = Visibility.Hidden;
                txtBlockStudentNameList[i].Visibility = Visibility.Hidden;
                txtBlockStudentTimeList[i].Visibility = Visibility.Hidden;
            }
            gridStudentAnswer.Visibility = Visibility.Hidden;
        }

        public void HideAll()
        {
            mediaAct.Stop(videoAnswer);
            mediaAct.Stop(videoIntro);
            mediaAct.Stop(videoRule);
            mediaAct.Stop(videoQuestion);
            mediaAct.Stop(videoQuestionStart);
            mediaAct.Stop(videoTime);

            mediaAct.Stop(soundFalseAnswer);
            mediaAct.Stop(soundHint);
            mediaAct.Stop(soundIntro);
            mediaAct.Stop(soundShowMatrix);
            mediaAct.Stop(soundTrueAnswer);
            mediaAct.Stop(soundTrueChoose);

            HideGridIntro();
            HideGridMatrix();
            HideGridQuestion();
            HideGridStudentAnswer();
            HideGridFinalAnswer();
        }

        void ThreadAnswer()
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            int x = (end.Hour * 3600000 + end.Minute * 60000 + end.Second * 1000 + end.Millisecond) - (start.Millisecond + start.Second * 1000 + start.Minute * 60000 + start.Hour * 3600000);
            while (x <= time * 1000)
            {
                if (x >= (numberOfStudent + 1) * 1000)
                    this.Dispatcher.Invoke(() =>
                    {
                        txtBlockStudentAnswerList[((x - 1000) / 1000) % numberOfStudent].Visibility = Visibility.Visible;
                        txtBlockStudentNameList[((x - 1000) / 1000) % numberOfStudent].Visibility = Visibility.Visible;
                        txtBlockStudentTimeList[((x - 1000) / 1000) % numberOfStudent].Visibility = Visibility.Visible;
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

        private void VideoAnswer_MediaEnded(object sender, RoutedEventArgs e)
        {
            //thread.Abort();
        }

        public void StartThreadAnswer()
        {
            time = 8;
            thread = new Thread(ThreadAnswer);
            thread.Start();
        }

        void ThreadHint()
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            double x = (end.Hour * 3600000 + end.Minute * 60000 + end.Second * 1000 + end.Millisecond) - (start.Millisecond + start.Second * 1000 + start.Minute * 60000 + start.Hour * 3600000);
            while (x <= time * 1000)
            {
                this.Dispatcher.Invoke(() =>
                {
                    txtBlockClock.Text = Math.Round(10 - x / 1000, 0).ToString();
                    if (txtBlockClock.Text.Length == 1)
                        txtBlockClock.Text = '0' + txtBlockClock.Text;
                });
                end = DateTime.Now;
                x = (end.Hour * 3600000 + end.Minute * 60000 + end.Second * 1000 + end.Millisecond) - (start.Millisecond + start.Second * 1000 + start.Minute * 60000 + start.Hour * 3600000);
            }
            this.Dispatcher.Invoke(() =>
            {
                txtBlockClock.Visibility = Visibility.Hidden;
            });
            thread.Abort();
        }

        public void StartThreadHint()
        {
            time = 10;
            txtBlockClock.Text = "10";
            txtBlockClock.Visibility = Visibility.Visible;
            thread = new Thread(ThreadHint);
            thread.Start();
        }

        private void VideoQuestionStart_MediaEnded(object sender, RoutedEventArgs e)
        {
            txtBlockQuestion1.Visibility = Visibility.Visible;
            txtBlockQuestion2.Visibility = Visibility.Visible;
            imgQuestion.Visibility = Visibility.Visible;
            videoQuestion.Visibility = Visibility.Visible;
        }
    }
}
