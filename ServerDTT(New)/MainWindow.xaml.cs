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
using ServerDTT_New_.User_Control;

namespace ServerDTT_New_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<TextBox> txtBoxStudentNameList = new List<TextBox>();
        public List<TextBox> txtBoxStudentPointList = new List<TextBox>();
        List<Student> studentList = new List<Student>();
        EWMainWindow eWMainWindow;
        EWPointSummarized eWPointSummarized;
        EWStart eWStart;
        EWObstacles eWObstacles;
        EWAccelerate eWAccelerate;
        EWDecode eWDecode;
        EWFinish eWFinish;

        UCStart uCStart;
        UCObstacles uCObstacles;
        UCAccelerate uCAccelerate;
        UCDecode uCDecode;
        UCFinish uCFinish;

        Server server;

        const int numberOfStudent = 4;
        public MainWindow()
        {
            InitializeComponent();
            InitMainWindow();
        }

        void InitMainWindow()
        {
            for (int i = 0; i < numberOfStudent; i++)
            {
                TextBox txtBoxName = new TextBox { FontSize = 25, Background = Brushes.LightBlue, Text = "TS" + (i + 1).ToString(), Margin = new Thickness(5), Width = 125, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, TextAlignment = TextAlignment.Center };
                txtBoxName.SetValue(Grid.RowProperty, i + 1);
                txtBoxName.SetValue(Grid.ColumnProperty, 0);
                gridStudentInformation.Children.Add(txtBoxName);
                txtBoxStudentNameList.Add(txtBoxName);

                TextBox txtBoxPoint = new TextBox { FontSize = 25, Background = Brushes.AliceBlue, Text = "0", Margin = new Thickness(5), Width = 125, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, TextAlignment = TextAlignment.Center };
                txtBoxPoint.SetValue(Grid.RowProperty, i + 1);
                txtBoxPoint.SetValue(Grid.ColumnProperty, 1);
                gridStudentInformation.Children.Add(txtBoxPoint);
                txtBoxStudentPointList.Add(txtBoxPoint);
                studentList.Add(new Student("TS" + (i + 1).ToString(), 0));
            }

            eWMainWindow = new EWMainWindow();
            eWMainWindow.Show();
            eWPointSummarized = new EWPointSummarized();
            eWStart = new EWStart();
            eWObstacles = new EWObstacles();
            eWAccelerate = new EWAccelerate();
            eWDecode = new EWDecode();
            eWFinish = new EWFinish();
            server = new Server(this);

            uCStart = new UCStart(this, eWMainWindow, eWStart, studentList, server);
            uCObstacles = new UCObstacles(this, eWMainWindow, eWObstacles, studentList, server);
            uCAccelerate = new UCAccelerate(this, eWMainWindow, eWAccelerate, studentList, server);
            uCDecode = new UCDecode(this, eWMainWindow, eWDecode, studentList, server);
            uCFinish = new UCFinish(this, eWMainWindow, eWFinish, studentList, server);

            tabMain.Items.Add(new TabItem { Content = uCStart, Header = "Khởi Động", Width = 80, Height = 20, FontSize = 10 });
            tabMain.Items.Add(new TabItem { Content = uCObstacles, Header = "VCNV", Width = 80, Height = 20, FontSize = 10 });
            tabMain.Items.Add(new TabItem { Content = uCAccelerate, Header = "Tăng Tốc", Width = 80, Height = 20, FontSize = 10 });
            tabMain.Items.Add(new TabItem { Content = uCDecode, Header = "Giải Mã", Width = 80, Height = 20, FontSize = 10 });
            tabMain.Items.Add(new TabItem { Content = uCFinish, Header = "Về Đích", Width = 80, Height = 20, FontSize = 10 });

            //Cap nhap ID cua vong hien tai
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("Round.txt");
            streamWriter.Flush();
            streamWriter.Write("0");
            streamWriter.Close();
        }

        public void SolveMessage(string message)
        {
            switch (message[0])
            {
                case '1':
                    break;
                case '2':
                    uCObstacles.SolveMessage(message.Substring(2));
                    break;
                case '3':
                    uCAccelerate.SolveMessage(message.Substring(2));
                    break;
                case '4':
                    uCFinish.SolveMessage(message.Substring(2));
                    break;
                case '5':
                    uCDecode.SolveMessage(message.Substring(2));
                    break;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < numberOfStudent; i++)
            {
                studentList[i].Name = txtBoxStudentNameList[i].Text;
                studentList[i].Point = int.Parse(txtBoxStudentPointList[i].Text);
            }
            uCStart.UpdateInfoOnScreen();
            uCObstacles.UpdateInfoOnScreen();
            uCAccelerate.UpdateInfoOnScreen();
            uCDecode.UpdateInfoOnScreen();
            uCFinish.UpdateInfoOnScreen();

            System.IO.StreamReader stream = new System.IO.StreamReader("Round.txt");
            int round = int.Parse(stream.ReadLine());
            stream.Close();
            server.SendTSInfo(round, studentList);
        }

        private void BtnSummarizedPoint_Click(object sender, RoutedEventArgs e)
        {
            eWMainWindow.Content = eWPointSummarized;
            eWPointSummarized.PlayVideoSummarizedPoint(studentList);
        }

        private void BtnFinal_Click(object sender, RoutedEventArgs e)
        {
            eWMainWindow.Content = eWPointSummarized;
            eWPointSummarized.soundFinishAll.Play();
        }
    }
}
