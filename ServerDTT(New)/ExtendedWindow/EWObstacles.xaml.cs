using ServerDTT_New_.SupportClass;
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

namespace ServerDTT_New_.ExtendedWindow
{
    /// <summary>
    /// Interaction logic for EWObstacles.xaml
    /// </summary>
    public partial class EWObstacles : UserControl
    {
        MediaAct mediaAct = new MediaAct();

        public EWObstacles()
        {
            InitializeComponent();
            InitEWControl();

        }

        private void InitEWControl()
        {
            ImageBrush imageBrush = new ImageBrush();
            mediaAct.Upload(imageBrush, "EW_Background.jpg");
            this.Background = imageBrush;

            //Media Source
            mediaAct.Upload(VideoIntro, "Obstacles_IntroVideo.mp4");
            mediaAct.Upload(VideoQuestionBox, "Obstacles_VideoLoadQuestionBox.mp4");
            mediaAct.Upload(VideoTiming, "Obstacles_VideoTiming.mp4");
            mediaAct.Upload(VideoShowAnswer, "Obstacles_VideoShowAnswer.mp4");

            mediaAct.Upload(imgShowAnswer, "Obstacles_ImageShowAnswer.png");
            mediaAct.Upload(imgQuestionBox, "Obstacles_ImageQuestionBox.png");
            mediaAct.Upload(imgObstacle, "Obstacles_Obstacle.jpg");
            mediaAct.Upload(imgHider1, "Obstacles_Hider1.png");
            mediaAct.Upload(imgHider2, "Obstacles_Hider2.png");
            mediaAct.Upload(imgHider3, "Obstacles_Hider3.png");
            mediaAct.Upload(imgHider4, "Obstacles_Hider4.png");
            mediaAct.Upload(imgHider5, "Obstacles_Hider5.png");
        }
    }
}