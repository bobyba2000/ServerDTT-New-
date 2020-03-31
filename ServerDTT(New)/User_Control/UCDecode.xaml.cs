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
using System.Windows.Media.Animation;
using ServerDTT_New_.DTO;
using ServerDTT_New_.ExtendedWindow;
using ServerDTT_New_.SupportClass;

namespace ServerDTT_New_.User_Control
{
    /// <summary>
    /// Interaction logic for UCDecode.xaml
    /// </summary>
    /// 

    public partial class UCDecode : UserControl
    {
        MainWindow mainWindow;
        EWDecode eWDecode;
        EWMainWindow eWMainWindow;
        List<Student> studentList;
        Server server;

        const int numberOfStudent = 4;
        int currentQuestionID = 0;
        int maxRowLength, maxColLength;
        int currentRow = 0, currentCol = 0;
        int currentStudentID = 0;
        int numberOfHint = 0;
        int currentTime = 0;

        string mainQuestion = "", mainAnswer = "";

        int IsQuestionOrHint = 1;//1 la question, 0 la hint

        List<DecodeQuestion> decodeQuestionList = new List<DecodeQuestion>();

        List<Button> btnStudentList = new List<Button>();
        List<TextBlock> txtBlockStudentAnswerList = new List<TextBlock>();
        List<TextBox> txtBoxStudentPointList = new List<TextBox>();
        List<Answer> answerList = new List<Answer>();
        List<Image> hintImageList = new List<Image>();

        Button[,] btnMatrixCellArray;
        int[,] timeMatrixCellArray;
        int[,] numberOfHintArray;

        MediaAct mediaAct = new MediaAct();

        public UCDecode(MainWindow main, EWMainWindow eWMain, EWDecode ewDecode, List<Student> _studentList, Server _server)
        {
            InitializeComponent();

            mainWindow = main;
            eWDecode = ewDecode;
            eWMainWindow = eWMain;
            studentList = _studentList;
            server = _server;

            InitUC();
        }

        void InitUC()
        {
            for (int i = 0; i < numberOfStudent; i++)
            {
                Button btnStudent = new Button { Background = Brushes.MidnightBlue, Foreground = Brushes.White, Content = "TS" + (i + 1).ToString(), Uid = i.ToString() };
                btnStudent.SetValue(Grid.ColumnProperty, 0);
                btnStudent.SetValue(Grid.RowProperty, i * 2);
                btnStudent.Click += BtnStudent_Click;
                btnStudentList.Add(btnStudent);
                gridStudentInfo.Children.Add(btnStudent);

                TextBlock txtBlockStudentAnswer = new TextBlock { Margin = new Thickness(3), TextAlignment = TextAlignment.Center, Background = Brushes.White };
                txtBlockStudentAnswer.SetValue(Grid.ColumnProperty, 1);
                txtBlockStudentAnswer.SetValue(Grid.RowProperty, i * 2);
                txtBlockStudentAnswerList.Add(txtBlockStudentAnswer);
                gridStudentInfo.Children.Add(txtBlockStudentAnswer);

                TextBox txtBoxStudentPoint = new TextBox { Margin = new Thickness(3), TextAlignment = TextAlignment.Center, Background = Brushes.White };
                txtBoxStudentPoint.SetValue(Grid.ColumnProperty, 2);
                txtBoxStudentPoint.SetValue(Grid.RowProperty, i * 2);
                txtBoxStudentPointList.Add(txtBoxStudentPoint);
                gridStudentInfo.Children.Add(txtBoxStudentPoint);
            }

            decodeQuestionList = DAO.DecodeQuestionDAO.Instance.getQuestions();
            maxColLength = decodeQuestionList[0].RowNo + 1;
            maxRowLength = decodeQuestionList[0].ColNo + 1;
            eWDecode.txtBlockMainQuestion.Text = mainQuestion = decodeQuestionList[0].Detail;
            eWDecode.txtBlockMainAnswer.Text = mainAnswer = decodeQuestionList[0].Answer;

            CreateMatrix();

            for (int i = 0; i < numberOfStudent; i++)
            {
                Answer answer = new Answer { studentID = i, answer = string.Empty, time = 0 };
                answerList.Add(answer);
            }
        }

        void CreateMatrix()
        {
            btnMatrixCellArray = new Button[maxRowLength, maxColLength];
            eWDecode.btnMatrixCellArray = new Button[maxRowLength, maxColLength];
            timeMatrixCellArray = new int[maxRowLength, maxColLength];
            numberOfHintArray = new int[maxRowLength, maxColLength];

            for (int i = 0; i <= maxColLength; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                ColumnDefinition eWColumnDefinition = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };

                eWDecode.gridMatrixTable.ColumnDefinitions.Add(eWColumnDefinition);
                gridMatrixTable.ColumnDefinitions.Add(columnDefinition);
            }

            for (int i = 0; i <= maxRowLength; i++)
            {
                RowDefinition rowDefinition = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
                RowDefinition eWRowDefinition = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };

                eWDecode.gridMatrixTable.RowDefinitions.Add(rowDefinition);
                gridMatrixTable.RowDefinitions.Add(eWRowDefinition);
            }

            for (int i = 1; i <= maxColLength; i++)
            {
                Viewbox viewboxColumnName = new Viewbox { Child = new TextBlock { Foreground = Brushes.White, FontFamily = new FontFamily("Open Sans"), Text = (char)(64 + i) + "" } };
                viewboxColumnName.SetValue(Grid.ColumnProperty, i);
                viewboxColumnName.SetValue(Grid.RowProperty, 0);
                gridMatrixTable.Children.Add(viewboxColumnName);

                Viewbox eWViewboxColumnName = new Viewbox { Child = new TextBlock { Foreground = Brushes.White, FontFamily = new FontFamily("Open Sans"), Text = (char)(64 + i) + "" } };
                eWViewboxColumnName.SetValue(Grid.ColumnProperty, i);
                eWViewboxColumnName.SetValue(Grid.RowProperty, 0);
                eWDecode.gridMatrixTable.Children.Add(eWViewboxColumnName);
            }

            for (int i = 1; i <= maxRowLength; i++)
            {
                Viewbox viewboxColumnName = new Viewbox { Child = new TextBlock { Foreground = Brushes.White, FontFamily = new FontFamily("Open Sans"), Text = i.ToString() } };
                viewboxColumnName.SetValue(Grid.ColumnProperty, 0);
                viewboxColumnName.SetValue(Grid.RowProperty, i);
                gridMatrixTable.Children.Add(viewboxColumnName);

                Viewbox eWViewboxColumnName = new Viewbox { Child = new TextBlock { Foreground = Brushes.White, FontFamily = new FontFamily("Open Sans"), Text = i.ToString() } };
                eWViewboxColumnName.SetValue(Grid.ColumnProperty, 0);
                eWViewboxColumnName.SetValue(Grid.RowProperty, i);
                eWDecode.gridMatrixTable.Children.Add(eWViewboxColumnName);
            }

            for (int i = 0; i < maxRowLength; i++)
                for (int j = 0; j < maxColLength; j++)
                {
                    btnMatrixCellArray[i, j] = new Button { BorderThickness = new Thickness(1.5), BorderBrush = Brushes.Black, Name = string.Format("Matrix_{0}_{1}", i, j), Background = Brushes.White, FontSize = 40, FontFamily = new FontFamily("Open Sans"), Content = string.Empty };
                    btnMatrixCellArray[i, j].SetValue(Grid.RowProperty, i + 1);
                    btnMatrixCellArray[i, j].SetValue(Grid.ColumnProperty, j + 1);
                    gridMatrixTable.Children.Add(btnMatrixCellArray[i, j]);

                    eWDecode.btnMatrixCellArray[i, j] = new Button { BorderThickness = new Thickness(1.5), BorderBrush = Brushes.Black, Name = string.Format("Matrix_{0}_{1}", i, j), Background = Brushes.White, FontSize = 40, FontFamily = new FontFamily("Open Sans"), Content = string.Empty };
                    eWDecode.btnMatrixCellArray[i, j].SetValue(Grid.RowProperty, i + 1);
                    eWDecode.btnMatrixCellArray[i, j].SetValue(Grid.ColumnProperty, j + 1);
                    eWDecode.gridMatrixTable.Children.Add(eWDecode.btnMatrixCellArray[i, j]);

                    timeMatrixCellArray[i, j] = 0;
                    numberOfHintArray[i, j] = 0;
                }

            Brush[] colorOfCell = { Brushes.Green, Brushes.Yellow, Brushes.Red };
            for (int i = 1; i < decodeQuestionList.Count; i++)
            {
                int difficulty = decodeQuestionList[i].QuestionTypeID / 10 - 1;
                int row = decodeQuestionList[i].RowNo;
                int col = decodeQuestionList[i].ColNo;

                btnMatrixCellArray[row, col].Background = colorOfCell[difficulty];
                eWDecode.btnMatrixCellArray[row, col].Background = colorOfCell[difficulty];
                if (decodeQuestionList[i].QuestionTypeID % 10 == 1)
                {
                    timeMatrixCellArray[row, col] = 15 + 5 * difficulty;
                    btnMatrixCellArray[row, col].Uid = "Decode_ImgQuestionIcon.png";
                }
                else
                {
                    timeMatrixCellArray[row, col] = 10;
                    numberOfHint++;
                    btnMatrixCellArray[row, col].Uid = "Decode_ImgHintIcon.png";

                    Image hintImage = new Image();
                    mediaAct.Upload(hintImage, decodeQuestionList[i].QuestionImageName);
                    hintImage.Stretch = Stretch.Fill;
                    hintImageList.Add(hintImage);
                    hintImage.Visibility = Visibility.Hidden;
                }

                btnMatrixCellArray[row, col].Click += BtnMatrixCell_Click;
            }

            SetUpNumberOfHint();
        }

        void SetUpNumberOfHint()
        {
            for (int row = 0; row < maxRowLength; row++)
                for (int col = 0; col < maxColLength; col++)
                {
                    numberOfHintArray[row, col] = 0;
                    for (int i = -1; i < 2; i++)
                        for (int j = -1; j < 2; j++)
                            if (!(row + i < 0 || row + i >= maxRowLength || col + j < 0 || col + j >= maxColLength || (i == 0 && j == 0)))
                            {
                                if (timeMatrixCellArray[row + i, col + j] == 10)
                                    numberOfHintArray[row, col]++;
                            }
                    if (btnMatrixCellArray[row, col].Background == Brushes.White)
                    {
                        btnMatrixCellArray[row, col].Content = numberOfHintArray[row, col].ToString();
                        eWDecode.btnMatrixCellArray[row, col].Content = numberOfHintArray[row, col].ToString();
                    }
                }
        }

        void BtnMatrixCell_Click(object sender,RoutedEventArgs e)
        {
            Button btnMatrixCell = sender as Button;
            string[] nameInfo = btnMatrixCell.Name.Split('_');
            currentRow = int.Parse(nameInfo[1]);
            currentCol = int.Parse(nameInfo[2]);

            if (timeMatrixCellArray[currentRow, currentCol] == 10)
            {
                IsQuestionOrHint = 0;
                mediaAct.Upload(eWDecode.videoTime, "Decode_Video15s.mp4");
                currentTime = 15;
            }
            else
            {
                IsQuestionOrHint = 1;
                mediaAct.Upload(eWDecode.videoTime, "Decode_Video" + timeMatrixCellArray[currentRow, currentCol].ToString() + "s.mp4");
                currentTime = timeMatrixCellArray[currentRow, currentCol];
            }

            if(btnMatrixCell.Content=="")
            {
                Image imgBtnIcon = new Image();
                mediaAct.Upload(imgBtnIcon, btnMatrixCell.Uid);
                btnMatrixCell.Content = imgBtnIcon;

                Image eWImgBtnIcon = new Image();
                mediaAct.Upload(eWImgBtnIcon, btnMatrixCell.Uid);
                eWDecode.btnMatrixCellArray[currentRow, currentCol].Content = eWImgBtnIcon;
            }
            else
            {
                mediaAct.Play(eWDecode.soundHint);
                btnMatrixCell.Content = eWDecode.btnMatrixCellArray[currentRow, currentCol].Content = numberOfHintArray[currentRow, currentCol].ToString();
            }
        }

        private void BtnIntroVideo_Click(object sender, RoutedEventArgs e)
        {
            eWMainWindow.Content = eWDecode;
            eWDecode.HideAll();
            eWDecode.gridIntro.Visibility = Visibility.Visible;
            mediaAct.Play(eWDecode.videoIntro);

            //Cap nhap ID cua vong hien tai
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("Round.txt");
            streamWriter.Flush();
            streamWriter.Write("5");
            streamWriter.Close();

            server.SendTSInfo(5, studentList);
        }

        private void BtnIntroSound_Click(object sender, RoutedEventArgs e)
        {
            eWDecode.HideAll();
            eWDecode.gridIntro.Visibility = Visibility.Visible;
            mediaAct.Play(eWDecode.soundIntro);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            eWDecode.HideAll();
            eWDecode.gridMatrix.Visibility = Visibility.Visible;
            mediaAct.Play(eWDecode.soundShowMatrix);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            //Trống vì matrix đã được lấy từ file sql;
        }

        private void BtnStudent_Click(object sender, RoutedEventArgs e)
        {
            currentStudentID = int.Parse((sender as Button).Uid);
        }

        private void BtnQuestionHint_Click(object sender, RoutedEventArgs e)
        {
            DecodeQuestion decodeQuestion = DAO.DecodeQuestionDAO.Instance.getQuestion(currentRow, currentCol);
            eWDecode.txtBlockQuestion2.Text = eWDecode.txtBlockQuestion1.Text = txtBlockQuestion.Text = decodeQuestion.Detail;
            mediaAct.Upload(eWDecode.imgQuestion, decodeQuestion.QuestionImageName);
            mediaAct.Upload(eWDecode.videoQuestion, decodeQuestion.QuestionVideoName);
            txtBlockAnswer.Text = decodeQuestion.Answer;

            if (decodeQuestion.QuestionImageName == "" && decodeQuestion.QuestionVideoName == "")
                eWDecode.txtBlockQuestion1.Text = "";
            else eWDecode.txtBlockQuestion2.Text = "";

            eWDecode.HideAll();
            eWDecode.gridQuestion.Visibility = Visibility.Visible;
            mediaAct.Play(eWDecode.videoQuestionStart);

            ResetAnswerList();
            ResetTxtBlockAnswerAndPoint();

            for (int i = 0; i < server.ClientList.Count; i++)
            {
                server.Send(server.ClientList[i], "5_1_" + decodeQuestion.Detail);
            }
        }

        private void BtnMatrix_Click(object sender, RoutedEventArgs e)
        {
            eWDecode.HideAll();
            eWDecode.gridMatrix.Visibility = Visibility.Visible;
        }

        private void BtnQuestionTime_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWDecode.videoTime);
            mediaAct.Play(eWDecode.videoQuestion);

            for (int i = 0; i < server.ClientList.Count; i++)
            {
                server.Send(server.ClientList[i], "5_2_" + currentTime.ToString());
            }
        }

        private void BtnDisable_Click(object sender, RoutedEventArgs e)
        {
            Image disableIcon = new Image();
            mediaAct.Upload(disableIcon, "Decode_ImgDisableIcon.png");
            btnMatrixCellArray[currentRow, currentCol].Content = disableIcon;

            Image eWDisableIcon = new Image();
            mediaAct.Upload(eWDisableIcon, "Decode_ImgDisableIcon.png");
            eWDecode.btnMatrixCellArray[currentRow, currentCol].Content = eWDisableIcon;

            mediaAct.Play(eWDecode.soundFalseAnswer);
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

        void ResetTxtBlockAnswerAndPoint()
        {
            for (int i = 0; i < numberOfStudent; i++)
            {
                txtBlockStudentAnswerList[i].Text = "";
                txtBoxStudentPointList[i].Text = "0";
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

        private void BtnStudentAnswer_Click(object sender, RoutedEventArgs e)
        {
            eWDecode.HideAll();
            eWDecode.gridStudentAnswer.Visibility = Visibility.Visible;

            answerList.Sort(CompareTimeAnswer);
            for (int i = 0; i < numberOfStudent; i++)
            {
                eWDecode.txtBlockStudentAnswerList[i].Text = answerList[i].answer;
                eWDecode.txtBlockStudentNameList[i].Text = studentList[answerList[i].studentID].Name;
                eWDecode.txtBlockStudentTimeList[i].Text = Math.Round(answerList[i].time, 2).ToString();
            }

            mediaAct.Play(eWDecode.videoAnswer);
            eWDecode.StartThreadAnswer();
        }

        private void BtnTrue_Click(object sender, RoutedEventArgs e)
        {
            if (IsQuestionOrHint == 1) mediaAct.Play(eWDecode.soundTrueAnswer);
            else mediaAct.Play(eWDecode.soundTrueChoose);
        }

        private void BtnFalse_Click(object sender, RoutedEventArgs e)
        {
            mediaAct.Play(eWDecode.soundFalseAnswer);
        }

        public void UpdateInfoOnScreen()
        {
            for (int i = 0; i < numberOfStudent; i++)
            {
                mainWindow.txtBoxStudentPointList[i].Text = studentList[i].Point.ToString();
                eWDecode.txtBlockStudentNameList[i].Text = studentList[i].Name;
                btnStudentList[i].Content = studentList[i].Name;
                eWDecode.txtBlockStudentAnswerList[i].Text = eWDecode.txtBlockStudentTimeList[i].Text = "";
            }
        }

        private void BtnSendPoint_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < numberOfStudent; i++)
                studentList[i].Point += int.Parse(txtBoxStudentPointList[i].Text);
            UpdateInfoOnScreen();
            server.SendTSInfo(5, studentList);
            ResetTxtBlockAnswerAndPoint();
        }

        private void BtnFinalHint_Click(object sender, RoutedEventArgs e)
        {
            eWDecode.HideAll();
            eWDecode.gridQuestion.Visibility = Visibility.Visible;
            eWDecode.txtBlockQuestion2.Text = txtBlockQuestion.Text = mainQuestion;
            eWDecode.txtBlockQuestion1.Text = "";
            eWDecode.imgQuestion.Source = null;
            eWDecode.videoQuestion.Source = null;

            txtBlockAnswer.Text = mainAnswer;
            currentTime = 15;
            mediaAct.Play(eWDecode.videoQuestionStart);

            mediaAct.Upload(eWDecode.videoTime, "Decode_Video15s.mp4");

            ResetAnswerList();
            ResetTxtBlockAnswerAndPoint();
        }

        void SetUpFinalAnswer()
        {
            for (int i = 0; i < 3; i++)
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(150, GridUnitType.Star) };
                eWDecode.gridFinalAnswer.RowDefinitions.Add(row);
            }
            for (int i = 0; i < (numberOfHint - 2) / 2; i++)
            {
                ColumnDefinition col = new ColumnDefinition { Width = new GridLength(800 * 1.0 / (numberOfHint - 2), GridUnitType.Star) };
                eWDecode.gridFinalAnswer.ColumnDefinitions.Add(col);
            }
            for (int i = 0; i < (numberOfHint - 2) / 2; i++)
            {
                hintImageList[i].SetValue(Grid.RowProperty, 0);
                hintImageList[i].SetValue(Grid.ColumnProperty, i);
                eWDecode.gridFinalAnswer.Children.Add(hintImageList[i]);
                hintImageList[(numberOfHint - 2) / 2 + 1 + i].SetValue(Grid.ColumnProperty, i);
                hintImageList[(numberOfHint - 2) / 2 + 1 + i].SetValue(Grid.RowProperty, 2);
                eWDecode.gridFinalAnswer.Children.Add(hintImageList[(numberOfHint - 2) / 2 + 1 + i]);
            }
            hintImageList[(numberOfHint - 2) / 2].SetValue(Grid.RowProperty, 1);
            hintImageList[(numberOfHint - 2) / 2].SetValue(Grid.ColumnProperty, (numberOfHint - 2) / 2);
            eWDecode.gridFinalAnswer.Children.Add(hintImageList[(numberOfHint - 2) / 2]);
            hintImageList[numberOfHint - 1].SetValue(Grid.RowProperty, 1);
            hintImageList[numberOfHint - 1].SetValue(Grid.ColumnProperty, 0);
            eWDecode.gridFinalAnswer.Children.Add(hintImageList[numberOfHint - 1]);

            eWDecode.viewBoxMainAnswer.SetValue(Grid.ColumnProperty, 1);
            eWDecode.viewBoxMainAnswer.SetValue(Grid.RowProperty, 1);
            eWDecode.viewBoxMainAnswer.SetValue(Grid.ColumnSpanProperty, (numberOfHint - 2) / 2 - 2);
        }

        private void BtnFinalAns_Click(object sender, RoutedEventArgs e)
        {
            SetUpFinalAnswer();
            eWDecode.HideAll();
            eWDecode.gridFinalAnswer.Visibility = Visibility.Visible;

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = 1;
            doubleAnimation.Duration = TimeSpan.FromSeconds(3);
            for (int i = 0; i < numberOfHint; i++)
            {
                hintImageList[i].Visibility = Visibility.Visible;
                hintImageList[i].BeginAnimation(OpacityProperty, doubleAnimation);
            }
            eWDecode.txtBlockMainAnswer.Visibility = Visibility.Visible;
            eWDecode.txtBlockMainAnswer.BeginAnimation(OpacityProperty, doubleAnimation);

            mediaAct.Play(eWDecode.soundTrueAnswer);
        }

        private void BtnChooseHintTime_Click(object sender, RoutedEventArgs e)
        {
            eWDecode.StartThreadHint();
        }

        private void BtnBUQuestion_Click(object sender, RoutedEventArgs e)
        {
            DecodeQuestion decodeQuestion = DAO.BUDecodeQuestionDAO.Instance.getQuestion(currentRow, currentCol);
            eWDecode.txtBlockQuestion2.Text = eWDecode.txtBlockQuestion1.Text = txtBlockQuestion.Text = decodeQuestion.Detail;
            mediaAct.Upload(eWDecode.imgQuestion, decodeQuestion.QuestionImageName);
            mediaAct.Upload(eWDecode.videoQuestion, decodeQuestion.QuestionVideoName);
            txtBlockAnswer.Text = decodeQuestion.Answer;

            if (decodeQuestion.QuestionImageName == "" && decodeQuestion.QuestionVideoName == "")
                eWDecode.txtBlockQuestion1.Text = "";
            else eWDecode.txtBlockQuestion2.Text = "";

            eWDecode.HideAll();
            eWDecode.gridQuestion.Visibility = Visibility.Visible;
            mediaAct.Play(eWDecode.videoQuestionStart);

            ResetAnswerList();
            ResetTxtBlockAnswerAndPoint();

            for (int i = 0; i < server.ClientList.Count; i++)
            {
                server.Send(server.ClientList[i], "5_1_" + decodeQuestion.Detail);
            }
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
            double time = Math.Round(currentTime - ConvertFromStringToDouble(messageList[2]), 2);
            string answer = messageList[3];

            txtBlockStudentAnswerList[StudentID].Text = answerList[StudentID].answer = answer;
            answerList[StudentID].time = time;
            answerList[StudentID].studentID = StudentID;
        }

        private void BtnRule_Click(object sender, RoutedEventArgs e)
        {
            eWDecode.HideAll();
            eWDecode.gridIntro.Visibility = Visibility.Visible;
            mediaAct.Play(eWDecode.videoRule);
        }

    }
}
