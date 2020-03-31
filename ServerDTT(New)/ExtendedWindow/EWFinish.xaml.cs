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

namespace ServerDTT_New_.ExtendedWindow
{
    /// <summary>
    /// Interaction logic for EWFinish.xaml
    /// </summary>
    public partial class EWFinish : UserControl
    {
        public List<TextBlock> txtBlockStudentNameList = new List<TextBlock>();
        public List<TextBlock> txtBlockStudentPointList = new List<TextBlock>();
        public List<Image> imgCheckMarkList = new List<Image>();
        public List<Border> borderHighlightList = new List<Border>();

        MediaAct mediaAct = new MediaAct();

        const int numberOfStudent = 4;
        const int numberOfQuestion = 9;

        public EWFinish()
        {
            ImageBrush imageBrush = new ImageBrush();
            mediaAct.Upload(imageBrush, "Background.jpg");
            this.Background = imageBrush;
            InitializeComponent();

            InitEW();
        }

        void InitEW()
        {
            //Tao ten Thi sinh
            for (int i = 0; i < numberOfStudent; i++)
            {
                TextBlock txtBlockStudentName = new TextBlock { Text = "Thi Sinh" + (i + 1).ToString(), FontWeight = FontWeights.DemiBold, FontFamily = new FontFamily("Open Sans"), Foreground = Brushes.White, FontSize = 25, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom};
                txtBlockStudentNameList.Add(txtBlockStudentName);
                //Viewbox viewboxStudentName = new Viewbox { Margin = new Thickness(5) };
                //viewboxStudentName.SetValue(Grid.ColumnProperty, i + 1);
                //viewboxStudentName.Child = txtBlockStudentName;
                //gridStudentsName.Children.Add(viewboxStudentName);
                txtBlockStudentName.SetValue(Grid.ColumnProperty, i + 1);
                txtBlockStudentName.SetValue(Grid.RowProperty, 0);
                txtBlockStudentName.SetValue(Grid.RowSpanProperty, 2);
                gridStudentsName.Children.Add(txtBlockStudentName);

                TextBlock txtBlockStudentPoint = new TextBlock {Margin=new Thickness(3), Text = "0", FontWeight = FontWeights.DemiBold, FontFamily = new FontFamily("Open Sans"), FontSize = 25, Foreground = Brushes.White, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Top};
                txtBlockStudentPoint.SetValue(Grid.ColumnProperty, i + 1);
                txtBlockStudentPoint.SetValue(Grid.RowProperty, 1);
                txtBlockStudentPoint.SetValue(Grid.RowSpanProperty, 3);
                txtBlockStudentPointList.Add(txtBlockStudentPoint);
                gridStudentsName.Children.Add(txtBlockStudentPoint);

                Border borderHighlight = new Border { Margin = new Thickness(3, 20, 3, 20), BorderBrush = Brushes.Yellow, BorderThickness=new Thickness(10), Background=Brushes.Transparent };
                borderHighlight.SetValue(Grid.RowProperty, 0);
                borderHighlight.SetValue(Grid.RowSpanProperty, 4);
                borderHighlight.SetValue(Grid.ColumnProperty, i + 1);
                borderHighlightList.Add(borderHighlight);
                gridStudentsName.Children.Add(borderHighlight);
            }
            
            //Tao cac Check Mark         
            for (int i = 0; i < numberOfQuestion; i++)
            {
                Image imgCheckMark = new Image();
                mediaAct.Upload(imgCheckMark, "Finish_ImageCheckMark.png");
                imgCheckMark.Visibility = Visibility.Visible;
                imgCheckMark.SetValue(Grid.RowProperty, i + 1 + (i / 3));
                imgCheckMark.SetValue(Grid.ColumnProperty, 1);
                imgCheckMarkList.Add(imgCheckMark);
                gridChooseQuestion.Children.Add(imgCheckMark);
            }
            //Upload cac file len
            mediaAct.Upload(imgHopeStar, "Finish_ImageHopeStar.png");
            mediaAct.Upload(imgTimeVideo, "Finish_ImageTimeVideo.png");

            mediaAct.Upload(videoChooseQuestion, "Finish_VideoChooseQuestion.mp4");
            mediaAct.Upload(videoIntro, "Finish_VideoIntro.mp4");

            mediaAct.Upload(soundFalse, "Finish_SoundFalse.mp3");
            mediaAct.Upload(soundFinish, "Finish_SoundFinish.wav");
            mediaAct.Upload(soundIntro, "Finish_SoundIntro.mp3");
            mediaAct.Upload(soundStart, "Finish_SoundStart.mp3");
            mediaAct.Upload(soundTrue, "Finish_SoundTrue.mp3");
            mediaAct.Upload(soundHopeStar, "Finish_SoundHopeStar.mp3");
            mediaAct.Upload(soundOtherStudentChance, "Finish_SoundOtherStudentChance.mp3");
            mediaAct.Upload(soundBell, "Finish_SoundBell.mp3");
            mediaAct.Upload(soundPackChosen, "Finish_SoundPackChosen.mp3");

        }

        void HideGridStudentContest()
        {
            gridStudentContest.Visibility = Visibility.Hidden;
            gridStudentsName.Visibility = Visibility.Hidden;

            for (int i = 0; i < numberOfQuestion; i++)
                imgCheckMarkList[i].Visibility = Visibility.Hidden;
            imgHopeStar.Visibility = Visibility.Hidden;
            imgQuestion.Visibility = Visibility.Hidden;
            imgTimeVideo.Visibility = Visibility.Hidden;
            
            videoTime.Visibility = Visibility.Hidden;
            videoChooseQuestion.Visibility = Visibility.Hidden;
            mediaQuestion.Visibility = Visibility.Hidden;

            txtBlockPackQuestion.Visibility = Visibility.Hidden;
            txtBlockPoint.Visibility = Visibility.Hidden;
            txtBlockQuestion.Visibility = Visibility.Hidden;
            txtBlockStudent.Visibility = Visibility.Hidden;
        }

        void HideIntro()
        {
            gridIntro.Visibility = Visibility.Hidden;
            videoIntro.Visibility = Visibility.Hidden;
        }

        public void HideAll()
        {
            mediaAct.Stop(soundBell);
            mediaAct.Stop(soundFalse);
            mediaAct.Stop(soundFinish);
            mediaAct.Stop(soundHopeStar);
            mediaAct.Stop(soundIntro);
            mediaAct.Stop(soundOtherStudentChance);
            mediaAct.Stop(soundPackChosen);
            mediaAct.Stop(soundStart);
            mediaAct.Stop(soundTrue);

            mediaAct.Stop(videoChooseQuestion);
            mediaAct.Stop(videoIntro);
            mediaAct.Stop(videoTime);

            mediaAct.Stop(mediaQuestion);

            HideGridStudentContest();
            HideIntro();
        }

        private void MediaQuestion_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaAct.Stop(mediaQuestion);
        }

        private void SoundStart_MediaEnded(object sender, RoutedEventArgs e)
        {
            gridStudentContest.Visibility = Visibility.Visible;
            txtBlockStudent.Visibility = Visibility.Visible;
            mediaAct.Play(videoChooseQuestion);
        }
    }
}
