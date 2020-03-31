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
using ServerDTT_New_.ExtendedWindow;
using ServerDTT_New_.SupportClass;
using ServerDTT_New_.DTO;


namespace ServerDTT_New_.User_Control
{
    /// <summary>
    /// Interaction logic for UCAccelerate.xaml
    /// </summary>

    class Answer
    {
        public string answer;
        public double time;
        public int studentID;
    }

    public partial class UCAccelerate : UserControl
    {
        MainWindow mainWindow;
        EWAccelerate eWAccelerate;
        EWMainWindow eWMainWindow;
        List<Student> studentList;
        Server server;

        List<Question> questionList = new List<Question>();
        List<Question> bUQuestionList = new List<Question>();
        List<Answer> answerList = new List<Answer>();

        List<TextBlock> txtBlockStudentNameList = new List<TextBlock>();
        List<TextBlock> txtBlockStudentAnswerList = new List<TextBlock>();
        List<TextBlock> txtBlockStudentTimeList = new List<TextBlock>();
        List<CheckBox> checkBoxTrueAnswerList = new List<CheckBox>();

        MediaAct mediaAct = new MediaAct();
        
        const int numberOfStudent = 4;
        int currentQuestionID = 0;
        
        public UCAccelerate(MainWindow main, EWMainWindow eWMain, EWAccelerate ewAccelerate, List<Student> _studentList, Server _server)
        {
            InitializeComponent();

            mainWindow = main;
            eWAccelerate = ewAccelerate;
            eWMainWindow = eWMain;
            studentList = _studentList;
            server = _server;

            InitUC();
        }

        void InitUC()
        {
            for (int i = 0; i < numberOfStudent; i++)
            {
                TextBlock txtBlockStudentName = new TextBlock { Margin = new Thickness(3), Text = "TS" + (i + 1).ToString(), FontSize = 20, Background = Brushes.Transparent, TextAlignment = TextAlignment.Center, HorizontalAlignment=HorizontalAlignment.Center };
                txtBlockStudentName.SetValue(Grid.ColumnProperty, 0);
                txtBlockStudentName.SetValue(Grid.RowProperty, i);
                txtBlockStudentNameList.Add(txtBlockStudentName);
                gridAnswerInfo.Children.Add(txtBlockStudentName);

                TextBlock txtBlockStudentAnswer = new TextBlock { Margin = new Thickness(3), FontSize = 20, Background = Brushes.White, TextAlignment = TextAlignment.Center, HorizontalAlignment=HorizontalAlignment.Center };
                txtBlockStudentAnswer.SetValue(Grid.ColumnProperty, 2);
                txtBlockStudentAnswer.SetValue(Grid.RowProperty, i);
                txtBlockStudentAnswerList.Add(txtBlockStudentAnswer);
                gridAnswerInfo.Children.Add(txtBlockStudentAnswer);

                TextBlock txtBlockStudentTime = new TextBlock { Margin = new Thickness(3), FontSize = 20, Background = Brushes.White, TextAlignment = TextAlignment.Center, HorizontalAlignment=HorizontalAlignment.Center };
                txtBlockStudentTime.SetValue(Grid.ColumnProperty, 1);
                txtBlockStudentTime.SetValue(Grid.RowProperty, i);
                txtBlockStudentTimeList.Add(txtBlockStudentTime);
                gridAnswerInfo.Children.Add(txtBlockStudentTime);

                CheckBox checkBoxTrueAnswer = new CheckBox { Background = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                checkBoxTrueAnswer.SetValue(Grid.ColumnProperty, 3);
                checkBoxTrueAnswer.SetValue(Grid.RowProperty, i);
                checkBoxTrueAnswerList.Add(checkBoxTrueAnswer);
                gridAnswerInfo.Children.Add(checkBoxTrueAnswer);
            }
            questionList = DAO.QuestionDAO.Instance.getAccelerateQuestion();
            bUQuestionList = DAO.BUQuestionDAO.Instance.getAccelerateQuestion();
            
            for (int i = 0; i < numberOfStudent; i++)
            {
                Answer answer = new Answer { studentID = i, answer = string.Empty, time = 0 };
                answerList.Add(answer);
            }

            eWAccelerate.server = server;
        }

        private void BtnIntroSound_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWAccelerate.soundIntro);
        }

        private void BtnIntroVideo_Click(object sender, RoutedEventArgs e)
        {
            eWMainWindow.Content = eWAccelerate;
            eWAccelerate.HideAll();
            eWAccelerate.gridIntro.Visibility = Visibility.Visible;
            mediaAct.Play(eWAccelerate.videoIntro);

            //Cap nhap ID cua vong hien tai
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("Round.txt");
            streamWriter.Flush();
            streamWriter.Write("3");
            streamWriter.Close();

            server.SendTSInfo(3, studentList);
        }

        void ResetAnswerList()
        {
            for (int i = 0; i < answerList.Count; i++)
            {
                answerList[i].answer = "";
                answerList[i].studentID = i;
                answerList[i].time = 0;
            }
        }

        void ResetTxtBlockAnswerAndTime()
        {
            for (int i = 0; i < numberOfStudent; i++)
                txtBlockStudentAnswerList[i].Text = txtBlockStudentTimeList[i].Text = "";
        }

        private void Question_Click(object sender, RoutedEventArgs e)
        {
            eWAccelerate.HideAll();
            eWAccelerate.gridQuestion.Visibility = Visibility.Visible;

            currentQuestionID = int.Parse((sender as Button).Uid);
            txtBlockAnswer.Text = questionList[currentQuestionID].Answer;
            txtBlockQuestion.Text = eWAccelerate.txtBlockQuestion.Text = questionList[currentQuestionID].Detail;
            mediaAct.Upload(eWAccelerate.imgQuestion, questionList[currentQuestionID].QuestionImageName);
            mediaAct.Upload(eWAccelerate.videoQuestion, questionList[currentQuestionID].QuestionVideoName);
            mediaAct.Play(eWAccelerate.videoQuestionStart);

            ResetAnswerList();
            ResetTxtBlockAnswerAndTime();
        }

        private void BtnTimeVideo_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWAccelerate.videoQuestion);

            mediaAct.Play(eWAccelerate.videoTime);
            eWAccelerate.imgQuestion.Visibility = Visibility.Visible;
            for (int i = 0; i < server.ClientList.Count; i++)
            {
                server.Send(server.ClientList[i], "3_1");
            }
        }

        int CompareTimeAnswer(Answer a, Answer b)
        {
            if (a.time == 0) return 1;
            if (b.time == 0) return -1;
            if (a.time > b.time)
                return 1;
            return -1;
        }

        private void BtnNameAnswer_Click(object sender, RoutedEventArgs e)
        {
            eWAccelerate.HideAll();
            eWAccelerate.gridStudentAnswer.Visibility = Visibility.Visible;
            answerList.Sort(CompareTimeAnswer);
            for (int i = 0; i < numberOfStudent; i++)
            {
                eWAccelerate.txtBlockStudentAnswerList[i].Text = answerList[i].answer;
                eWAccelerate.txtBlockStudentNameList[i].Text = studentList[answerList[i].studentID].Name;
                eWAccelerate.txtBlockStudentTimeList[i].Text = Math.Round(answerList[i].time, 2).ToString();
            }
            mediaAct.Play(eWAccelerate.videoAnswer);
            eWAccelerate.StartThreadAnswer();
        }

        int CompareTrueAnswerTime(Answer a, Answer b)
        {
            if (!(bool)checkBoxTrueAnswerList[a.studentID].IsChecked)
                return 1;
            if ((bool)checkBoxTrueAnswerList[b.studentID].IsChecked && a.time > b.time)
                return 1;
            return -1;
        }

        public void UpdateInfoOnScreen()
        {
            for (int i = 0; i < numberOfStudent; i++)
            {
                mainWindow.txtBoxStudentPointList[i].Text = studentList[i].Point.ToString();
                txtBlockStudentNameList[i].Text = eWAccelerate.txtBlockStudentNameList[i].Text = studentList[i].Name;
                eWAccelerate.txtBlockStudentAnswerList[i].Text = eWAccelerate.txtBlockStudentTimeList[i].Text = "";
            }
        }

        void CheckAnswerAndAddPoint()
        {
            answerList.Sort(CompareTrueAnswerTime);
            for (int i = 0; i < 4; i++)
                if (checkBoxTrueAnswerList[answerList[i].studentID].IsChecked == true)
                    studentList[answerList[i].studentID].Point += 40 - 10 * i;
            UpdateInfoOnScreen();
            server.SendTSInfo(3, studentList);
        }

        private void BtnTrue_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWAccelerate.soundTrue);
            CheckAnswerAndAddPoint();
        }

        private void BtnFalse_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWAccelerate.soundFalse);
        }

        double ConvertFromStringToDouble(string number)
        {
            double x = 2.6;
            char decimalPoint = x.ToString()[1];
            string result = string.Empty;
            for (int i = 0; i < number.Length; i++)
                if (number[i] < '0' || number[i] > '9')
                    result += decimalPoint;
                else result += number[i];
            return double.Parse(result);
        }

        public void SolveMessage(string message)
        {
            string[] messageList = message.Split('_');
            int StudentID = int.Parse(messageList[0]);
            double time = Math.Round(30 - ConvertFromStringToDouble(messageList[1]), 2);
            string answer = messageList[2];

            txtBlockStudentAnswerList[StudentID].Text = answerList[StudentID].answer = answer;
            answerList[StudentID].time = time;
            txtBlockStudentTimeList[StudentID].Text = time.ToString();
            answerList[StudentID].studentID = StudentID;
        }

        private void BtnShowAnswer_Click(object sender, RoutedEventArgs e)
        {
            eWAccelerate.HideAll();
            eWAccelerate.gridQuestion.Visibility = Visibility.Visible;
            eWAccelerate.videoQuestionStart.Visibility = Visibility.Visible;

            eWAccelerate.imgQuestion.Source = null;
            eWAccelerate.imgQuestion.Visibility = Visibility.Visible;

            eWAccelerate.videoQuestion.Source = null;
            eWAccelerate.videoQuestionStart.Visibility = Visibility.Visible;

            mediaAct.Upload(eWAccelerate.imgQuestion, questionList[currentQuestionID].AnswerImageName);
            mediaAct.Upload(eWAccelerate.videoQuestion, questionList[currentQuestionID].AnswerVideoName);
        }

        private void BtnBUQuestion_Click(object sender, RoutedEventArgs e)
        {
            eWAccelerate.HideAll();
            eWAccelerate.gridQuestion.Visibility = Visibility.Visible;

            txtBlockAnswer.Text = bUQuestionList[currentQuestionID].Answer;
            txtBlockQuestion.Text = eWAccelerate.txtBlockQuestion.Text = bUQuestionList[currentQuestionID].Detail;
            mediaAct.Upload(eWAccelerate.imgQuestion, bUQuestionList[currentQuestionID].QuestionImageName);
            mediaAct.Upload(eWAccelerate.videoQuestion, bUQuestionList[currentQuestionID].QuestionVideoName);
            mediaAct.Play(eWAccelerate.videoQuestionStart);

            ResetAnswerList();
            ResetTxtBlockAnswerAndTime();
        }
    }
}
