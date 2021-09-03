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

namespace SinBot.Views
{
    /// <summary>
    /// Interaction logic for VideoPanel.xaml
    /// </summary>
    public partial class SmallVideoPanel : Window
    {
        public SmallVideoPanel()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Top - this.Height;

            SessionContext.SmallVideoPanel = myMediaElement;
            SessionContext.SmallVideoPanelWindow = this;
        }

        private void myMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Top - this.Height;

            SessionContext.SmallVideoPanel.Source = new Uri("E:\\SinBot\\SinBot\\MediaFiles\\Video\\Misc\\RESET.mp4");
        }
    }
}
