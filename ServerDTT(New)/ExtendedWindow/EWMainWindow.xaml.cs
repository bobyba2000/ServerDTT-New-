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
using System.Windows.Shapes;
using ServerDTT_New_.SupportClass;

namespace ServerDTT_New_.ExtendedWindow
{
    /// <summary>
    /// Interaction logic for EWMainWindow.xaml
    /// </summary>
    public partial class EWMainWindow : Window
    {
        MediaAct mediaAct = new MediaAct();
        public EWMainWindow()
        {
            ImageBrush imageBrush = new ImageBrush();
            mediaAct.Upload(imageBrush, "Background.jpg");
            this.Background = imageBrush;
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowStyle = WindowStyle.None;
        }
    }
}
