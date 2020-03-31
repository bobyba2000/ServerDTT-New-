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
using ServerDTT_New_.ExtendedWindow;

namespace ServerDTT_New_.User_Control
{
    /// <summary>
    /// Interaction logic for UC_Start.xaml
    /// </summary>
    public partial class UCStart : UserControl
    {
        MediaAct mediaAct = new MediaAct();
        List<Button> btnStudentList = new List<Button>();
        List<DTO.Question> questionList = new List<DTO.Question>();
        List<DTO.Question> bUQuestionList = new List<DTO.Question>();

        MainWindow mainWindow;
        EWMainWindow eWMainWindow;
        EWStart eWStart;
        List<Student> studentList;
        Server server;

        DTO.Question currentQuestion;

        int currentStudentID = 0;
        int currentQuestionID = 0;

        bool IsBackup = false;
        int currentBUQuestionID = 0;

        const int numberOfStudent = 4;
        const int numberOfQuestion = 12;
        int idStartQuestion = 0;
        int idEndQuestion = 0;
        public UCStart(MainWindow main, EWMainWindow ewMainWindow, EWStart ewStart, List<Student> students, Server _server)
        {
            InitializeComponent();

            studentList = students;
            mainWindow = main;
            eWStart = ewStart;
            server = _server;
            eWMainWindow = ewMainWindow;
            InitUC();
        }

        void InitUC()
        {
            for (int i = 0; i < numberOfStudent; i++)
            {
                Button btnStudent = new Button {Margin=new Thickness(5), Content = "TS" + (i+1).ToString(), Uid=i.ToString(), Background = Brushes.Blue, FontSize = 20, FontWeight = FontWeights.DemiBold };
                btnStudent.SetValue(Grid.RowProperty, i + 1);
                btnStudent.Click += BtnStudent_Click;
                btnStudentList.Add(btnStudent);
                gridBtnStudent.Children.Add(btnStudent);
            }

            questionList = DAO.QuestionDAO.Instance.getStartQuestion();
            bUQuestionList = DAO.BUQuestionDAO.Instance.getStartQuestion();
            eWStart.UpdateUC(this);
        }


        private void BtnIntroSound_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWStart.soundIntro);
        }

        private void BtnIntroVideo_Click(object sender, RoutedEventArgs e)
        {
            eWMainWindow.Content = eWStart;
            eWStart.HideAll();
            eWStart.gridIntro.Visibility = Visibility.Visible;
            mediaAct.Play(eWStart.videoIntro);

            server.SendTSInfo(1, studentList);
        }


        private void BtnStudent_Click(object sender, RoutedEventArgs e)
        {
            currentStudentID = int.Parse((sender as Button).Uid);
            idStartQuestion = currentStudentID * numberOfQuestion;

            eWStart.HideAll();
            eWStart.gridStudentContest.Visibility = Visibility.Visible;
            eWStart.txtBlockStudent.Text = studentList[currentStudentID].Name;
            eWStart.txtBlockStudent.Visibility = Visibility.Visible;

            eWStart.txtBlockStudentNameList[currentStudentID].FontSize = 35;
            eWStart.txtBlockStudentNameList[currentStudentID].VerticalAlignment = VerticalAlignment.Center;
            eWStart.txtBlockStudentPointList[currentStudentID].Text = "";
            eWStart.txtBlockStudentNameList[currentStudentID].SetValue(Grid.RowSpanProperty, 3);
        }


        private void BtnStudentStart_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWStart.soundStart);
            eWStart.txtBlockPoint.Text = studentList[currentStudentID].Point.ToString();
            eWStart.txtBlockStudentNameList[currentStudentID].Text = studentList[currentStudentID].Name;
        }

        public void ShowNextQuestion(int IsCorrect)
        {
            if (idEndQuestion <= idStartQuestion + numberOfQuestion)
            {
                if (currentQuestionID < questionList.Count && IsBackup == false)
                {
                    currentQuestion = questionList[currentQuestionID];
                    currentQuestionID++;
                }

                else if (currentBUQuestionID < bUQuestionList.Count)
                {
                    currentQuestion = bUQuestionList[currentBUQuestionID];
                    currentBUQuestionID++;
                    idEndQuestion--;
                }

                eWStart.txtBlockQuestion.Text = txtBlockQuestion.Text = currentQuestion.Detail;
                txtBlockAnswer.Text = currentQuestion.Answer;
                eWStart.mediaQuestion.Source = null;
                eWStart.imgQuestion.Source = null;
                if (currentQuestion.QuestionImageName != string.Empty)
                    mediaAct.Upload(eWStart.imgQuestion, currentQuestion.QuestionImageName);
                if (currentQuestion.QuestionVideoName != string.Empty)
                    mediaAct.Upload(eWStart.mediaQuestion, currentQuestion.QuestionVideoName);
                eWStart.imgQuestion.Visibility = Visibility.Visible;
                eWStart.mediaQuestion.Visibility = Visibility.Visible;

                string message = "1_1_" + IsCorrect.ToString() + "_" + currentQuestion.Detail + "_" + currentQuestion.QuestionImageName + currentQuestion.QuestionVideoName;
                for (int i = 0; i < server.ClientIDList.Count(); i++)
                    if (server.ClientIDList[i] == currentStudentID)
                    {
                        server.Send(server.ClientList[i], message);
                        break;
                    }

                idEndQuestion++;
            }
        }


        private void BtnTrueAnswer_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWStart.soundTrue);
            studentList[currentStudentID].Point += 10;
            eWStart.txtBlockPoint.Text = studentList[currentStudentID].Point.ToString();
            mainWindow.txtBoxStudentPointList[currentStudentID].Text = studentList[currentStudentID].Point.ToString();
            ShowNextQuestion(1);

            server.SendTSInfo(1, studentList);
        }

        private void BtnFalseAnswer_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWStart.soundFalse);
            ShowNextQuestion(0);
        }

        public void UpdateInfoOnScreen()
        {
            for (int i = 0; i < numberOfStudent; i++)
            {
                eWStart.txtBlockStudentNameList[i].Text = studentList[i].Name;
                eWStart.txtBlockStudentPointList[i].Text = studentList[i].Point.ToString();
                btnStudentList[i].Content = studentList[i].Name;
            }
        }

        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            eWStart.HideAll();
            mediaAct.Play(eWStart.soundFinish);
            UpdateInfoOnScreen();

            eWStart.txtBlockStudentNameList[currentStudentID].SetValue(Grid.RowSpanProperty, 2);
            eWStart.txtBlockStudentNameList[currentStudentID].FontSize = 30;
            eWStart.txtBlockStudentPointList[currentStudentID].Text = studentList[currentStudentID].Point.ToString();
            eWStart.txtBlockStudentNameList[currentStudentID].VerticalAlignment = VerticalAlignment.Bottom;

            IsBackup = false;

            idStartQuestion = idEndQuestion;
        }

        private void BtnBackupQuestion_Click(object sender, RoutedEventArgs e)
        {
            IsBackup = true;
        }
    }
}
