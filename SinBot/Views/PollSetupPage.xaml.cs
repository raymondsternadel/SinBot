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
    /// Interaction logic for PollSetupPage.xaml
    /// </summary>
    public partial class PollSetupPage : Page
    {
        public PollSetupPage()
        {
            InitializeComponent();
        }

        private void TXTNewChoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && TXTNewChoice.Text != string.Empty)
            {
                var style = (Style)Application.Current.TryFindResource("PollButtonTheme");

                var button = new Button()
                {
                    Height = 50,
                    Margin = new Thickness(10, 10, 10, 10),
                    Content = TXTNewChoice.Text,
                    FontSize = 35,
                    Foreground = Brushes.White,
                    Background = (Brush)(new BrushConverter().ConvertFrom("#8f0101")),
                    Style = style
                };

                button.Click += Button_Click;

                SPPollChoices.Children.Add(button);
                TXTNewChoice.Text = string.Empty;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            SPPollChoices.Children.Remove(button);
        }
    }
}
