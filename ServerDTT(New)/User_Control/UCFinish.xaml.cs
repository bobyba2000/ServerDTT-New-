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
using ServerDTT_New_.DTO;
using ServerDTT_New_.ExtendedWindow;
using ServerDTT_New_.SupportClass;

namespace ServerDTT_New_.User_Control
{
    /// <summary>
    /// Interaction logic for UCFinish.xaml
    /// </summary>
    public partial class UCFinish : UserControl
    {
        MainWindow mainWindow;
        EWFinish eWFinish;
        EWMainWindow eWMainWindow;
        List<Student> studentList;
        Server server;

        List<TextBox> txtBoxChoosePackQuestionList = new List<TextBox>();
        public List<Button> btnChooseStudentList = new List<Button>();

        List<Question> questionList;
        List<Question> bUQuestionList;
        List<int> pointList = new List<int>();

        int currentQuestionID = 0;
        int currentStudentID = 0;
        int currentBellStudentID = -1;

        int IsHopeStar = 0;

        const int numberOfStudent = 4;
        const int numberOfQuestion = 3;

        MediaAct mediaAct = new MediaAct();

        public UCFinish(MainWindow main, EWMainWindow eWMain, EWFinish ewFinish, List<Student> _studentList, Server _server)
        {
            InitializeComponent();
            mainWindow = main;
            eWFinish = ewFinish;
            eWMainWindow = eWMain;
            studentList = _studentList;
            server = _server;

            InitUC();
        }

        void InitUC()
        {
            for (int i = 0; i < numberOfQuestion; i++)
            {
                TextBox txtBoxChoosePackQuestion = new TextBox { Background = Brushes.White, Foreground = Brushes.Black, TextAlignment = TextAlignment.Center, Text = "10", FontSize = 20 };
                Viewbox viewboxChoosePackQuestion = new Viewbox();
                viewboxChoosePackQuestion.SetValue(Grid.ColumnProperty, i);
                viewboxChoosePackQuestion.SetValue(Grid.RowProperty, 1);
                viewboxChoosePackQuestion.Child=txtBoxChoosePackQuestion;
                txtBoxChoosePackQuestionList.Add(txtBoxChoosePackQuestion);
                gridChooseQuestionPack.Children.Add(viewboxChoosePackQuestion);

                Viewbox viewboxPackQuestion = new Viewbox();
                viewboxPackQuestion.SetValue(Grid.ColumnProperty, i);
                Button btnPackQuestion = new Button { Background = Brushes.CornflowerBlue, Foreground = Brushes.Black, Content = "Câu hỏi" + (i + 1).ToString(), Uid = i.ToString() };
                viewboxPackQuestion.Child = btnPackQuestion;
                btnPackQuestion.Click += BtnPackQuestion_Click;
                gridPackQuestion.Children.Add(viewboxPackQuestion);
            }

            for (int i = 0; i < numberOfStudent; i++)
            {
                Button btnChooseStudent = new Button { Background = Brushes.LightBlue, Foreground = Brushes.Black, Content = "Thí sinh" + (i + 1).ToString(), Uid = i.ToString() };
                btnChooseStudent.Click += BtnChooseStudent_Click;
                Viewbox viewboxChooseStudent = new Viewbox();
                viewboxChooseStudent.SetValue(Grid.RowProperty, 2 + i);
                viewboxChooseStudent.Child = btnChooseStudent;
                gridChooseStudent.Children.Add(viewboxChooseStudent);
                btnChooseStudentList.Add(btnChooseStudent);
            }

        }

        private void BtnIntroVideo_Click(object sender, RoutedEventArgs e)
        {
            eWMainWindow.Content = eWFinish;
            eWFinish.HideAll();
            eWFinish.gridIntro.Visibility = Visibility.Visible;
            mediaAct.Play(eWFinish.videoIntro);

            //Cap nhap thong tin thi sinh
            UpdateInfoOnScreen();

            //Cap nhap ID cua vong hien tai
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("Round.txt");
            streamWriter.Flush();
            streamWriter.Write("4");
            streamWriter.Close();

            server.SendTSInfo(4, studentList);
        }

        private void BtnIntroSound_Click(object sender, RoutedEventArgs e)
        {
            eWFinish.HideAll();
            eWFinish.gridIntro.Visibility = Visibility.Visible;
            mediaAct.Play(eWFinish.soundIntro);
        }

        void BtnChooseStudent_Click(object sender, RoutedEventArgs e)
        {
            eWFinish.HideAll();
            eWFinish.gridChooseQuestion.Visibility = Visibility.Visible;
            currentStudentID = int.Parse((sender as Button).Uid);
            eWFinish.txtBlockStudent.Text = studentList[currentStudentID].Name;
            eWFinish.txtBlockStudent.Visibility = Visibility.Visible;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWFinish.soundStart);
            
        }

        private void BtnSaveQuestionPack_Click(object sender, RoutedEventArgs e)
        {
            pointList.Clear();

            int q1Difficulty = int.Parse(txtBoxChoosePackQuestionList[0].Text);
            int q2Difficulty = int.Parse(txtBoxChoosePackQuestionList[1].Text);
            int q3Difficulty = int.Parse(txtBoxChoosePackQuestionList[2].Text);

            if(q1Difficulty%10!=0||q2Difficulty%10!=0||q3Difficulty%10!=0||Math.Max(q1Difficulty,Math.Max(q2Difficulty,q3Difficulty))>30||Math.Min(q1Difficulty,Math.Min(q2Difficulty,q3Difficulty))<10)
            {
                MessageBox.Show("Lỗi nhập sai! Vui lòng nhập lại", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            questionList = DAO.QuestionDAO.Instance.getFinishQuestions(currentStudentID, q1Difficulty, q2Difficulty, q3Difficulty);
            bUQuestionList = DAO.BUQuestionDAO.Instance.getFinishQuestions(currentStudentID, q1Difficulty, q2Difficulty, q3Difficulty);

            for (int i = 0; i < 9; i++)
            {
                eWFinish.imgCheckMarkList[i].Visibility = Visibility.Hidden;
            }

            eWFinish.imgCheckMarkList[(q1Difficulty / 10 - 1) * 3].Visibility = Visibility.Visible;
            pointList.Add(q1Difficulty);

            eWFinish.imgCheckMarkList[(q2Difficulty / 10 - 1) * 3 + 1].Visibility = Visibility.Visible;
            pointList.Add(q2Difficulty);

            eWFinish.imgCheckMarkList[(q3Difficulty / 10 - 1) * 3 + 2].Visibility = Visibility.Visible;
            pointList.Add(q3Difficulty);

            mediaAct.Play(eWFinish.soundPackChosen);
        }

        void BtnPackQuestion_Click(object sender, RoutedEventArgs e)
        {
            eWFinish.HideAll();
            eWFinish.gridStudentContest.Visibility = Visibility.Visible;
            ResetQuestion();
            UpdateInfoOnScreen();
            currentQuestionID = int.Parse((sender as Button).Uid);

            txtBlockQuestion.Text = eWFinish.txtBlockQuestion.Text = questionList[currentQuestionID].Detail;
            txtBlockAnswer.Text = questionList[currentQuestionID].Answer;
            eWFinish.txtBlockPackQuestion.Text = "Câu hỏi " + pointList[currentQuestionID].ToString() + " điểm";
            eWFinish.txtBlockPoint.Text = studentList[currentStudentID].Point.ToString();
            mediaAct.Upload(eWFinish.mediaQuestion, questionList[currentQuestionID].QuestionVideoName);
            mediaAct.Upload(eWFinish.imgQuestion, questionList[currentQuestionID].QuestionImageName);

            mediaAct.Upload(eWFinish.videoTime, "Finish_Video" + (pointList[currentQuestionID] - 5 * pointList[currentQuestionID] / 10 + 5).ToString() + "s.mp4");
            eWFinish.txtBlockPackQuestion.Visibility = Visibility.Visible;
            eWFinish.txtBlockPoint.Visibility = Visibility.Visible;
            eWFinish.gridStudentsName.Visibility = Visibility.Visible;
            eWFinish.txtBlockQuestion.Visibility = Visibility.Visible;
            eWFinish.imgQuestion.Visibility = Visibility.Visible;
            eWFinish.imgTimeVideo.Visibility = Visibility.Visible;

            if (IsHopeStar == 1)
            {
                eWFinish.imgHopeStar.Visibility = Visibility.Visible;
            }
            for (int i = 0; i < server.ClientList.Count(); i++)
                server.Send(server.ClientList[i], "4_1_" + questionList[currentQuestionID].Detail);

            if (eWFinish.mediaQuestion.Source != null)
                btnPlayMedia.IsEnabled = true;
        }

        private void BtnPlayMedia_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWFinish.mediaQuestion);
        }

        private void BtnStartTime_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWFinish.videoTime);
        }

        private void BtnHopeStar_Click(object sender, RoutedEventArgs e)
        {
            IsHopeStar = 1;
            mediaAct.Play(eWFinish.soundHopeStar);
            eWFinish.imgHopeStar.Visibility = Visibility.Visible;
        }

        public void UpdateInfoOnScreen()
        {
            eWFinish.txtBlockPoint.Text = studentList[currentStudentID].Point.ToString();
            for (int i = 0; i < numberOfStudent; i++)
            {
                mainWindow.txtBoxStudentNameList[i].Text = eWFinish.txtBlockStudentNameList[i].Text = studentList[i].Name;
                mainWindow.txtBoxStudentPointList[i].Text = studentList[i].Point.ToString();

                btnChooseStudentList[i].Content = studentList[i].Name;
                eWFinish.txtBlockStudentNameList[i].SetValue(Grid.RowSpanProperty, 2);
                if (i != currentStudentID)
                    eWFinish.txtBlockStudentPointList[i].Text = studentList[i].Point.ToString();
                else
                {
                    eWFinish.txtBlockStudentNameList[i].FontSize = 35;
                    eWFinish.txtBlockStudentNameList[i].VerticalAlignment = VerticalAlignment.Center;
                    eWFinish.txtBlockStudentPointList[i].Text = "";
                    eWFinish.txtBlockStudentNameList[i].SetValue(Grid.RowSpanProperty, 4);
                }
            }
            
        }

        void ResetQuestion()
        {
            eWFinish.mediaQuestion.Source = null;
            eWFinish.imgQuestion.Source = null;

            btnPlayMedia.IsEnabled = false;

            currentBellStudentID = -1;
            for (int i = 0; i < numberOfStudent; i++)
            {
                eWFinish.borderHighlightList[i].Visibility = Visibility.Hidden;
                btnChooseStudentList[i].Background = Brushes.LightBlue;
            }

            IsHopeStar = 0;
        }

        private void BtnTrue_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWFinish.soundTrue);
            if(currentBellStudentID!=-1)
            {
                studentList[currentBellStudentID].Point += pointList[currentQuestionID];
                studentList[currentStudentID].Point -= pointList[currentQuestionID] - IsHopeStar * pointList[currentQuestionID];
                UpdateInfoOnScreen();
                server.SendTSInfo(4, studentList);
                currentBellStudentID = -1;

                IsHopeStar = 0;
                eWFinish.imgHopeStar.Visibility = Visibility.Hidden;
                return;
            }

            studentList[currentStudentID].Point += pointList[currentQuestionID] + IsHopeStar * pointList[currentQuestionID];
            UpdateInfoOnScreen();


            IsHopeStar = 0;
            eWFinish.imgHopeStar.Visibility = Visibility.Hidden;
            server.SendTSInfo(4, studentList);
        }

        private void BtnFalse_Click(object sender, RoutedEventArgs e)
        {
            if(currentBellStudentID==-1)
            {
                mediaAct.Play(eWFinish.soundOtherStudentChance);
                studentList[currentStudentID].Point -= IsHopeStar * pointList[currentQuestionID];
                UpdateInfoOnScreen();
                server.SendTSInfo(4, studentList);
                for (int i = 0; i < server.ClientList.Count; i++)
                {
                    server.Send(server.ClientList[i], "4_2_5");
                }
                
                eWFinish.imgHopeStar.Visibility = Visibility.Hidden;
                return;
            }

            mediaAct.Play(eWFinish.soundFalse);
            studentList[currentBellStudentID].Point -= pointList[currentQuestionID] / 2;
            UpdateInfoOnScreen();


            IsHopeStar = 0;
            eWFinish.imgHopeStar.Visibility = Visibility.Hidden;
            server.SendTSInfo(4, studentList);
        }

        public void SolveMessage(string message)
        {
            if (currentBellStudentID == -1)
            {
                currentBellStudentID = int.Parse(message[0] + "");
                mediaAct.Play(eWFinish.soundBell);
                btnChooseStudentList[int.Parse(message[0] + "")].Background = Brushes.Red;
                eWFinish.borderHighlightList[int.Parse(message[0] + "")].Visibility = Visibility.Visible;
            }
        }

        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            eWFinish.HideAll();
            mediaAct.Play(eWFinish.soundFinish);
            ResetQuestion();

            eWFinish.txtBlockStudentNameList[currentStudentID].SetValue(Grid.RowSpanProperty, 2);
            eWFinish.txtBlockStudentNameList[currentStudentID].FontSize = 25;
            eWFinish.txtBlockStudentPointList[currentStudentID].Text = studentList[currentStudentID].Point.ToString();
            eWFinish.txtBlockStudentNameList[currentStudentID].VerticalAlignment = VerticalAlignment.Bottom;

            
            IsHopeStar = 0;
            eWFinish.imgHopeStar.Visibility = Visibility.Hidden;
        }

        private void BtnBUQuestion_Click(object sender, RoutedEventArgs e)
        {
            eWFinish.HideAll();
            eWFinish.gridStudentContest.Visibility = Visibility.Visible;
            ResetQuestion();
            UpdateInfoOnScreen();

            txtBlockQuestion.Text = eWFinish.txtBlockQuestion.Text = bUQuestionList[currentQuestionID].Detail;
            txtBlockAnswer.Text = bUQuestionList[currentQuestionID].Answer;
            eWFinish.txtBlockPackQuestion.Text = "Câu hỏi " + pointList[currentQuestionID].ToString() + " điểm";
            eWFinish.txtBlockPoint.Text = studentList[currentStudentID].Point.ToString();
            mediaAct.Upload(eWFinish.mediaQuestion, bUQuestionList[currentQuestionID].QuestionVideoName);
            mediaAct.Upload(eWFinish.imgQuestion, bUQuestionList[currentQuestionID].QuestionImageName);

            mediaAct.Upload(eWFinish.videoTime, "Finish_Video" + (pointList[currentQuestionID] - 5 * pointList[currentQuestionID] / 10 + 5).ToString() + "s.mp4");
            eWFinish.txtBlockPackQuestion.Visibility = Visibility.Visible;
            eWFinish.txtBlockPoint.Visibility = Visibility.Visible;
            eWFinish.gridStudentsName.Visibility = Visibility.Visible;
            eWFinish.txtBlockQuestion.Visibility = Visibility.Visible;
            eWFinish.imgQuestion.Visibility = Visibility.Visible;
            eWFinish.imgTimeVideo.Visibility = Visibility.Visible;

            if (IsHopeStar == 1)
            {
                eWFinish.imgHopeStar.Visibility = Visibility.Visible;
            }
            for (int i = 0; i < server.ClientList.Count(); i++)
                server.Send(server.ClientList[i], "4_1_" + bUQuestionList[currentQuestionID].Detail);

            if (eWFinish.mediaQuestion.Source != null)
                btnPlayMedia.IsEnabled = true;
        }
    }
}
