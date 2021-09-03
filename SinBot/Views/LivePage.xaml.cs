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

namespace SinBot.Views
{
    /// <summary>
    /// Interaction logic for LivePage.xaml
    /// </summary>
    public partial class LivePage : Page
    {
        public LivePage()
        {
            InitializeComponent();
        }

        private void btnChat_Click(object sender, RoutedEventArgs e)
        {
            if (btnChat.Background == Brushes.Green)
            {
                btnChat.Background = Brushes.Red;
                SessionContext.ChatPanelWindow.Visibility = Visibility.Hidden;
                SessionContext.ChatVisible = false;
            }
            else
            {
                btnChat.Background = Brushes.Green;
                SessionContext.ChatPanelWindow.Visibility = Visibility.Visible;
                SessionContext.ChatVisible = true;
            }
        }

        private void btnWebcam_Click(object sender, RoutedEventArgs e)
        {
            if (btnWebcam.Background == Brushes.Green)
            {
                btnWebcam.Background = Brushes.Red;
                SessionContext.WebcamWindow.Visibility = Visibility.Hidden;
                SessionContext.WebcamVisible = false;
            }
            else
            {
                btnWebcam.Background = Brushes.Green;
                SessionContext.WebcamWindow.Visibility = Visibility.Visible;
                SessionContext.WebcamVisible = true;
            }
        }
    }
}
