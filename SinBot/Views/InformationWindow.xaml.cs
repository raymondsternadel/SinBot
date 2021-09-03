using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SinBot.Views
{
    /// <summary>
    /// Interaction logic for InformationWindow.xaml
    /// </summary>
    public partial class InformationWindow : Window
    {
        private Timer InformationWindowShowing;

        private Storyboard storyboard;
        public InformationWindow()
        {
            InitializeComponent();

            InformationWindowShowing = new Timer(20000);
            InformationWindowShowing.Elapsed += InformationWindowShowing_Elapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right / 2 - 400;
            this.Top = desktopWorkingArea.Bottom;

            SessionContext.InformationalPopupTimer.Elapsed += InformationalPopupTimer_Elapsed;

            var myAnimation = new DoubleAnimation();
            myAnimation.From = desktopWorkingArea.Bottom;
            myAnimation.To = desktopWorkingArea.Bottom - 225;
            myAnimation.FillBehavior = FillBehavior.Stop;

            myAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

            storyboard = new Storyboard();
            storyboard.Children.Add(myAnimation);

            Storyboard.SetTarget(myAnimation, this);
            Storyboard.SetTargetProperty(myAnimation, new PropertyPath(Window.TopProperty));
        }

        private async void InformationalPopupTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var message = await DatabaseService.GetRandomInformationalMessage();

            App.Current.Dispatcher.Invoke(() =>
            {
                TXTInfoPopup.Text = message.Message;

                storyboard.Begin(this);
                MEInfoSound.Position = TimeSpan.Zero;
                MEInfoSound.Play();
            });
            InformationWindowShowing.Stop();
            InformationWindowShowing.Start();
        }

        private void InformationWindowShowing_Elapsed(object sender, ElapsedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;

            App.Current.Dispatcher.Invoke(() =>
            {
                storyboard.Stop();
                WinInformationWindow.Top = desktopWorkingArea.Bottom;
            });
        }
    }
}
