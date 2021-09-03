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
    /// Interaction logic for FullScreenVideoPanel.xaml
    /// </summary>
    public partial class FullScreenVideoPanel : Window
    {
        public FullScreenVideoPanel()
        {
            InitializeComponent();
        }

        private void FullScreenMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            FullScreenMediaElement.Position = TimeSpan.Zero;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SessionContext.FullscreenWindow = this;
            SessionContext.FullscreenMediaElement = FullScreenMediaElement;
        }
    }
}
