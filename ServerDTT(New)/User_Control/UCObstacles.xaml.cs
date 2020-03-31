using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ServerDTT_New_.ExtendedWindow;
using ServerDTT_New_.SupportClass;
using ServerDTT_New_.DTO;
using System.Windows.Media.Animation;

namespace ServerDTT_New_.User_Control
{
    #region Network Code
    //This is network code
    //2_0 ~ SendTSInfo Update point, name
    //2_1 Send question
    //2_2 Start timing

    #endregion Network Code

    public partial class UCObstacles : UserControl
    {
        MainWindow Main;
        EWObstacles eWObstacles;
        EWMainWindow eWindow;

        List<Student> studentList = new List<Student>();
        List<Question> questionList = new List<Question>();
        List<TextBlock> eWObstacles_textboxAnswer = new List<TextBlock>();
        List<Border> eWObstacles_textboxBorderBell = new List<Border>();
        List<TextBlock> eWObstacles_textboxBell = new List<TextBlock>();
        List<Image> eWObstacles_imgHider = new List<Image>();
        List<List<Image>> eWObstacles_imgRowHider = new List<List<Image>>();
        List<Grid> eWObstacles_gridRowList = new List<Grid>();

        List<TextBlock> textboxBell = new List<TextBlock>();
        List<TextBox> textboxAns = new List<TextBox>();
        List<CheckBox> checkbox = new List<CheckBox>();
        TextBlock[][] Words = new TextBlock[4][];

        int bellOrder = 0;
        int currentQuestion = 1;
        int rowChosenCount = 0;
        bool isFinished = false; // To mute the showHintSound when the KEY is answered
        const int currentRound = 2;
        bool isBackup = false;

        Server server;
        MediaAct mediaAct = new MediaAct();

        public UCObstacles(MainWindow main, EWMainWindow eWMainWindow, EWObstacles _eWObstacles, List<Student> students, Server _server)
        {
            InitializeComponent();

            this.Main = main;
            eWObstacles = _eWObstacles;
            eWindow = eWMainWindow;
            studentList = students;
            server = _server;

            InitControl();
        }

        //1. The top part
        private void btnIntroVideo_Click(object sender, RoutedEventArgs e)
        {
            eWindow.Content = eWObstacles;
            eWObstacles.VideoIntro.Visibility = Visibility.Visible;
            eWObstacles.VideoIntro.Stop();
            eWObstacles.VideoIntro.Play();
            eWObstacles.VideoIntro.MediaEnded += VideoIntro_MediaEnded;

            server.SendTSInfo(currentRound, studentList);
        }

        private void btnIntroMusic_Click(object sender, RoutedEventArgs e)
        {
            IntroMusic.Stop();
            IntroMusic.Play();
        }

        private void btnShowRows_Click(object sender, RoutedEventArgs e)
        {
            eWObstacles.gridKeyImage.Visibility = Visibility.Hidden;
            eWObstacles.gridShowAns.Visibility = Visibility.Hidden;
            eWObstacles.textboxContent.Visibility = Visibility.Hidden;
            eWObstacles.VideoShowAnswer.Visibility = Visibility.Hidden;

            eWObstacles.gridRows.Visibility = Visibility.Visible;
            eWObstacles.gridUpperPart.Visibility = Visibility.Visible;

            ShowRowsSound.Play();
        }

        private void btnChooseRow_Click(object sender, RoutedEventArgs e)
        {
            //UC update part
            rowChosenCount++;
            currentQuestion = (int)comboBoxNumber.SelectedItem;

            textboxAns1.Text = String.Empty;
            textboxAns2.Text = String.Empty;
            textboxAns3.Text = String.Empty;
            textboxAns4.Text = String.Empty;

            RowChosenSound.Stop();
            RowChosenSound.Play();

            textboxQuestion.Text = questionList[currentQuestion].Detail;
            textboxTrueAnswer.Text = questionList[currentQuestion].Answer;

            //EW update part
            if (currentQuestion < 5)
                for (int i = 0; i < eWObstacles_imgRowHider[currentQuestion - 1].Count; i++)
                {
                    DoubleAnimation doubleAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(2));  //toValue == 0, hide the base element
                    eWObstacles_imgRowHider[currentQuestion - 1][i].BeginAnimation(Image.OpacityProperty, doubleAnimation);
                }

            eWObstacles.VideoQuestionBox.Stop();
            eWObstacles.VideoQuestionBox.Play();
            eWObstacles.VideoQuestionBox.MediaEnded += eWObstacles_ShowQuestion;

            EraseEWContent();

            //Send Question Detail
            for (int i = 0; i < server.ClientList.Count; i++)
                server.Send(server.ClientList[i], "2_1_" + questionList[currentQuestion].Detail);
        }

        private void btnStartTiming_Click(object sender, RoutedEventArgs e)
        {
            eWObstacles.VideoTiming.Visibility = Visibility.Visible;  //Assume that this video appear on the top layer otherwise you should control the Visi of imgQuestBox
            eWObstacles.VideoTiming.MediaEnded += VideoTiming_MediaEnded;   //Hide the video
            eWObstacles.VideoTiming.Stop();
            eWObstacles.VideoTiming.Play();


            //replica Enable btn Send
            for (int i = 0; i < server.ClientList.Count; i++)
                server.Send(server.ClientList[i], "2_2");
        }

        private void btnShowAnswer_Click(object sender, RoutedEventArgs e)
        {
            //Change the EW's background from QuestionBox Background -> Original Background
            ImageBrush imageBrush = new ImageBrush();
            mediaAct.Upload(imageBrush, "EW_Background.jpg");
            eWObstacles.Background = imageBrush;

            //Hide The Rows and Key Layers
            eWObstacles.gridUpperPart.Visibility = Visibility.Hidden;
            eWObstacles.gridLowerPart.Visibility = Visibility.Hidden;
            eWObstacles.imgQuestionBox.Visibility = Visibility.Hidden;
            eWObstacles.VideoTiming.Visibility = Visibility.Hidden;
            eWObstacles.gridKeyImage.Visibility = Visibility.Hidden;

            //Show The answer grid
            eWObstacles.VideoShowAnswer.Stop();
            eWObstacles.VideoShowAnswer.Play();
            eWObstacles.VideoShowAnswer.Visibility = Visibility.Visible;
            eWObstacles.VideoShowAnswer.MediaEnded += eWObstacles_ShowAnswer;

        }

        private void btnAddPoint_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestion < 5) ShowWords(currentQuestion);

            bool isFalse = true;    //To make sure that the true SOUND will NOT be PLAYED when ALL FALSE
            for (int i = 0; i < checkbox.Count(); i++)
                if (checkbox[i].IsChecked == true)
                {
                    isFalse = false;
                    studentList[i].Point += 10;
                    Main.txtBoxStudentPointList[i].Text = studentList[i].Point.ToString();
                }

            if (isFalse == false)
            {
                TrueRowSound.Stop();
                TrueRowSound.Play();
            }

            EraseContent();

            //replica update POINT
            server.SendTSInfo(currentRound, studentList);
        }

        private void btnShowHint_Click(object sender, RoutedEventArgs e)
        {
            eWObstacles.gridRows.Visibility = Visibility.Hidden;
            eWObstacles.gridShowAns.Visibility = Visibility.Hidden;
            eWObstacles.gridKeyImage.Visibility = Visibility.Visible;
            eWObstacles_imgHider[currentQuestion - 1].Visibility = Visibility.Hidden; //You mustn't set the index of the comboBox here because it will enable you to open 2 hints at a time

            if (isFinished != true) //If the KEY is answered, mute this sound
            {
                ShowHintSound.Stop();
                ShowHintSound.Play();
            }
        }
        //2. The right part
        private void btnShowAllHint_Click(object sender, RoutedEventArgs e)
        {
            //Open all rows and obstacle hider
            for (int i = 1; i <= 5; i++)
            {
                if (i < 5)
                {
                    ShowWords(i);

                    //Hide the GREY rowHider
                    for (int j = 0; j < eWObstacles_imgRowHider[i - 1].Count; j++)    
                        eWObstacles_imgRowHider[i - 1][j].Visibility = Visibility.Hidden;
                }

                eWObstacles_imgHider[i - 1].Visibility = Visibility.Hidden;
            }

            //Hide the QUESION BOX
            ImageBrush imageBrush = new ImageBrush();
            mediaAct.Upload(imageBrush, "EW_Background.jpg");
            eWObstacles.Background = imageBrush;

            isFinished = true;  //mute the ShowHintSound

            HideBell();
            eWObstacles.gridRows.Visibility = Visibility.Visible;
            eWObstacles.gridUpperPart.Visibility = Visibility.Visible;
            eWObstacles.gridLowerPart.Visibility = Visibility.Hidden;
            eWObstacles.gridShowAns.Visibility = Visibility.Hidden;
            eWObstacles.gridKeyImage.Visibility = Visibility.Hidden;
            eWObstacles.imgQuestionBox.Visibility = Visibility.Hidden;
        }

        private void btnEliminate_Click(object sender, RoutedEventArgs e)
        {
            EliminateSound.Stop();
            EliminateSound.Play();
            HideBell();

            //The replica's btnBell is already disabled, so don't have to send anything
        }

        private void btnAddKeyPoint_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < checkbox.Count(); i++)
                if (checkbox[i].IsChecked == true)
                {
                    studentList[i].Point += getKeyPoint();
                    Main.txtBoxStudentPointList[i].Text = studentList[i].Point.ToString();
                }

            TrueKeySound.Stop();
            TrueKeySound.Play();

            isFinished = true;  //mute the ShowHintSound
            EraseContent();
            HideBell();

            //replica update POINT
            server.SendTSInfo(currentRound, studentList);
        }

        //3. Attached Processings
        private void VideoIntro_MediaEnded(object sender, RoutedEventArgs e)
        {
            eWObstacles.VideoIntro.Visibility = Visibility.Hidden;
        }

        private void VideoTiming_MediaEnded(object sender, RoutedEventArgs e)
        {
            eWObstacles.VideoTiming.Visibility = Visibility.Hidden;
        }

        private int getActualWordLength(int currentQuest)
        {
            int length = questionList[currentQuest].Answer.Length;

            for (int j = 0; j < questionList[currentQuest].Answer.Length; j++)
            {
                if (questionList[currentQuest].Answer[j].Equals(' '))
                    length -= 1;
            }

            return length;
        }

        private void eWObstacles_ShowQuestion(object sender, RoutedEventArgs e)
        {
            //Media Ended of QuestionBoxVideo
            eWObstacles.VideoQuestionBox.Visibility = Visibility.Hidden;
            eWObstacles.imgQuestionBox.Visibility = Visibility.Visible;
            eWObstacles.gridLowerPart.Visibility = Visibility.Visible;
            eWObstacles.imgQuestionBox.Visibility = Visibility.Visible;
            eWObstacles.textboxQuestion.Text = questionList[currentQuestion].Detail;
        }

        private void eWObstacles_ShowAnswer(object sender, RoutedEventArgs e)
        {
            eWObstacles.VideoShowAnswer.Visibility = Visibility.Hidden;
            eWObstacles.gridRows.Visibility = Visibility.Hidden;
            eWObstacles.imgShowAnswer.Visibility = Visibility.Visible;   //Avoid preload this image after 1 question
            eWObstacles.gridShowAns.Visibility = Visibility.Visible;
        }

        public void ShowWords(int currentQuestion)
        {
            int length = 0;
            int curr = 0;

            while (curr < questionList[currentQuestion].Answer.Length)
            {
                if (questionList[currentQuestion].Answer[curr].Equals(' ') != true)
                {
                    Words[currentQuestion - 1][length].FontSize = 35;
                    Words[currentQuestion - 1][length].FontWeight = FontWeights.Bold;
                    Words[currentQuestion - 1][length].Text = questionList[currentQuestion].Answer[curr].ToString();
                    Words[currentQuestion - 1][length].Visibility = Visibility.Visible;
                    length++;
                }
                curr++;
            }
        }

        private int getKeyPoint()
        {
            switch (rowChosenCount)
            {
                case 1:
                    return 80;
                case 2:
                    return 60;
                case 3:
                    return 40;
                case 4:
                    return 20;
                case 5:
                    return 10;
                default:
                    return 80; //when no row is opened
            }
        }

        private void HideBell()
        {
            bellOrder = 0;
            for (int i = 0; i < textboxBell.Count(); i++)
            {
                textboxBell[i].Visibility = Visibility.Hidden;
                eWObstacles_textboxBorderBell[i].Visibility = Visibility.Hidden;
            }
        }

        private void EraseContent()
        {
            foreach (TextBox box in textboxAns) box.Text = String.Empty;
            foreach (CheckBox box in checkbox) box.IsChecked = false;
            textboxQuestion.Text = String.Empty;
            textboxTrueAnswer.Text = String.Empty;
        }

        private void EraseEWContent()
        {
            foreach (TextBlock box in eWObstacles_textboxAnswer) box.Text = String.Empty;
        }

        public void UpdateInfoOnScreen()
        {
            eWObstacles.textboxName1.Text = studentList[0].Name;
            eWObstacles.textboxName2.Text = studentList[1].Name;
            eWObstacles.textboxName3.Text = studentList[2].Name;
            eWObstacles.textboxName4.Text = studentList[3].Name;

            textboxName1.Text = studentList[0].Name;
            textboxName2.Text = studentList[1].Name;
            textboxName3.Text = studentList[2].Name;
            textboxName4.Text = studentList[3].Name;
        }

        //4. Network Handler
        public void SolveMessage(string message)
        {
            string[] messages = message.Split('_');
            int position = int.Parse(messages[0]);

            switch (messages[1])
            {
                case "1":
                    AnswerHandler(position, messages[2]);
                    break;
                case "0":
                    BellHandler(position);
                    break;
            }
        }

        public void AnswerHandler(int pos, string ans)
        {
            textboxAns[pos].Text = ans;
            eWObstacles_textboxAnswer[pos].Text = ans;
        }

        public void BellHandler(int pos)
        {
            eWObstacles_textboxBell[bellOrder].Text = studentList[pos].Name;
            eWObstacles_textboxBorderBell[bellOrder].Visibility = Visibility.Visible;

            bellOrder++;
            textboxBell[pos].Text = bellOrder.ToString();
            textboxBell[pos].Visibility = Visibility.Visible;

            BellSound.Stop();
            BellSound.Play();
        }

        //5.Init
        private void InitControl()
        {
            //Media Source
            mediaAct.Upload(IntroMusic, "Obstacles_IntroMusic.mp3");
            mediaAct.Upload(ShowRowsSound, "Obstacles_ShowRowsSound.mp3");
            mediaAct.Upload(RowChosenSound, "Obstacles_RowChosenSound.mp3");
            mediaAct.Upload(TrueRowSound, "Obstacles_TrueRowSound.mp3");
            mediaAct.Upload(ShowHintSound, "Obstacles_ShowHintSound.mp3");
            mediaAct.Upload(TrueKeySound, "Obstacles_TrueKeySound.mp3");
            mediaAct.Upload(EliminateSound, "Obstacles_EliminateSound.mp3");
            mediaAct.Upload(BellSound, "Obstacles_BellSound.mp3");

            comboBoxNumber.Items.Clear();
            textboxAns.Clear();
            checkbox.Clear();
            textboxBell.Clear();
            eWObstacles_textboxBell.Clear();
            eWObstacles_textboxBorderBell.Clear();
            eWObstacles_imgHider.Clear();
            for (int i = 0; i < eWObstacles_gridRowList.Count; i++)
                eWObstacles_gridRowList[i].Children.Clear();
            eWObstacles_gridRowList.Clear();

            comboBoxNumber.Items.Add(1);
            comboBoxNumber.Items.Add(2);
            comboBoxNumber.Items.Add(3);
            comboBoxNumber.Items.Add(4);
            comboBoxNumber.Items.Add(5);

            textboxName1.Text = studentList[0].Name;
            textboxName2.Text = studentList[1].Name;
            textboxName3.Text = studentList[2].Name;
            textboxName4.Text = studentList[3].Name;
            
            textboxAns.Add(textboxAns1);
            textboxAns.Add(textboxAns2);
            textboxAns.Add(textboxAns3);
            textboxAns.Add(textboxAns4);

            checkbox.Add(checkbox1);
            checkbox.Add(checkbox2);
            checkbox.Add(checkbox3);
            checkbox.Add(checkbox4);

            textboxBell.Add(textboxBell1);
            textboxBell.Add(textboxBell2);
            textboxBell.Add(textboxBell3);
            textboxBell.Add(textboxBell4);

            eWObstacles_textboxBell.Add(eWObstacles.textboxBell1);
            eWObstacles_textboxBell.Add(eWObstacles.textboxBell2);
            eWObstacles_textboxBell.Add(eWObstacles.textboxBell3);
            eWObstacles_textboxBell.Add(eWObstacles.textboxBell4);

            eWObstacles_textboxBorderBell.Add(eWObstacles.borderBell1);
            eWObstacles_textboxBorderBell.Add(eWObstacles.borderBell2);
            eWObstacles_textboxBorderBell.Add(eWObstacles.borderBell3);
            eWObstacles_textboxBorderBell.Add(eWObstacles.borderBell4);

            eWObstacles.textboxName1.Text = studentList[0].Name;
            eWObstacles.textboxName2.Text = studentList[1].Name;
            eWObstacles.textboxName3.Text = studentList[2].Name;
            eWObstacles.textboxName4.Text = studentList[3].Name;

            eWObstacles_textboxAnswer.Add(eWObstacles.textboxAnswer1);
            eWObstacles_textboxAnswer.Add(eWObstacles.textboxAnswer2);
            eWObstacles_textboxAnswer.Add(eWObstacles.textboxAnswer3);
            eWObstacles_textboxAnswer.Add(eWObstacles.textboxAnswer4);

            eWObstacles_imgHider.Add(eWObstacles.imgHider1);
            eWObstacles_imgHider.Add(eWObstacles.imgHider2);
            eWObstacles_imgHider.Add(eWObstacles.imgHider3);
            eWObstacles_imgHider.Add(eWObstacles.imgHider4);
            eWObstacles_imgHider.Add(eWObstacles.imgHider5);

            for (int i = 0; i < eWObstacles_imgHider.Count; i++)
                eWObstacles_imgHider[i].Visibility = Visibility.Visible;

            eWObstacles_gridRowList.Add(eWObstacles.gridRow1);
            eWObstacles_gridRowList.Add(eWObstacles.gridRow2);
            eWObstacles_gridRowList.Add(eWObstacles.gridRow3);
            eWObstacles_gridRowList.Add(eWObstacles.gridRow4);

            mediaAct.Upload(eWObstacles.imgRowNumber1, "Obstacles_RowNumberImage.png");
            mediaAct.Upload(eWObstacles.imgRowNumber2, "Obstacles_RowNumberImage.png");
            mediaAct.Upload(eWObstacles.imgRowNumber3, "Obstacles_RowNumberImage.png");
            mediaAct.Upload(eWObstacles.imgRowNumber4, "Obstacles_RowNumberImage.png");

            eWObstacles_imgRowHider.Clear();

            //Create Rows
            if (isBackup == false)
                questionList = DAO.QuestionDAO.Instance.getObstacleQuestion();
            else questionList = DAO.BUQuestionDAO.Instance.getObstacleQuestion();

            for (int i = 0; i < 4; i++)
            {
                int length = getActualWordLength(i + 1);
                eWObstacles_gridRowList[i].ColumnDefinitions.Clear();

                //Partition the Row grid cell 
                for (int j = 0; j < Math.Max(length, 15); j++)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition { Width = new GridLength(30, GridUnitType.Star) };
                    eWObstacles_gridRowList[i].ColumnDefinitions.Add(columnDefinition);
                }

                List<Image> images = new List<Image>();
                Words[i] = new TextBlock[length];

                for (int j = 0; j < length; j++)
                {
                    Image imgRowHider = new Image { Margin = new Thickness(3) };

                    imgRowHider.SetValue(Grid.ColumnProperty, j + (Math.Max(length, 15) - length) / 2);
                    mediaAct.Upload(imgRowHider, "Obstacles_RowHiderImage.png");
                    imgRowHider.Stretch = Stretch.Fill;
                    imgRowHider.Visibility = Visibility.Visible;
                    images.Add(imgRowHider);

                    Image imgRowChosen = new Image { Margin = new Thickness(3) };
                    imgRowChosen.SetValue(Grid.ColumnProperty, j + (Math.Max(length, 15) - length) / 2);
                    mediaAct.Upload(imgRowChosen, "Obstacles_RowChosenImage.png");
                    imgRowChosen.Stretch = Stretch.Fill;
                    imgRowChosen.Visibility = Visibility.Visible;

                    Words[i][j] = new TextBlock { Margin = new Thickness(0) };
                    Words[i][j].Background = Brushes.Transparent;
                    Words[i][j].Foreground = Brushes.White;
                    Words[i][j].FontSize = 165;
                    Words[i][j].VerticalAlignment = VerticalAlignment.Center;
                    Words[i][j].HorizontalAlignment = HorizontalAlignment.Center;
                    Words[i][j].Visibility = Visibility.Visible;
                    Words[i][j].SetValue(Grid.ColumnProperty, j + (Math.Max(length, 15) - length) / 2);

                    eWObstacles_gridRowList[i].Children.Add(imgRowChosen);
                    eWObstacles_gridRowList[i].Children.Add(Words[i][j]);
                    eWObstacles_gridRowList[i].Children.Add(imgRowHider);
                }

                eWObstacles_imgRowHider.Add(images);
            }

        }

        private void BtnBackUpQuestion_Click(object sender, RoutedEventArgs e)
        {
            isBackup = true;
            InitControl();
        }
    }
}