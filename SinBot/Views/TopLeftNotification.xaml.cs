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
    /// Interaction logic for TopLeftNotification.xaml
    /// </summary>
    public partial class TopLeftNotification : Window
    {
        private Timer TopLeftNotificationShowing;
        private Storyboard storyboard;
        private List<string> newFollowers;

        public TopLeftNotification()
        {
            InitializeComponent();

            TopLeftNotificationShowing = new Timer(5000);
            TopLeftNotificationShowing.Elapsed += TopLeftNotificationShowing_Elapsed;

            SessionContext.TwitchFollowerService.OnNewFollowersDetected += FollowerService_OnNewFollowersDetected;
            newFollowers = new List<string>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Left - this.Width;
            this.Top = desktopWorkingArea.Top + 50;

            var myAnimation = new DoubleAnimation();
            myAnimation.From = desktopWorkingArea.Left - this.Width;
            myAnimation.To = desktopWorkingArea.Left + 50;
            myAnimation.FillBehavior = FillBehavior.Stop;

            myAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            storyboard = new Storyboard();
            storyboard.Children.Add(myAnimation);

            Storyboard.SetTarget(myAnimation, this);
            Storyboard.SetTargetProperty(myAnimation, new PropertyPath(Window.LeftProperty));
        }

        private void TopLeftNotificationShowing_Elapsed(object sender, ElapsedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;

            App.Current.Dispatcher.Invoke(() =>
            {
                storyboard.Stop();
                WinTopLeftNotification.Left = desktopWorkingArea.Left - this.Width;
            });

        }

        private async void FollowerService_OnNewFollowersDetected(object sender, TwitchLib.Api.Services.Events.FollowerService.OnNewFollowersDetectedArgs e)
        {
            foreach (var follower in SessionContext.TwitchFollowerService.KnownFollowers)
            {
                var count = follower.Value.Count();

                if (count != e.NewFollowers.Count)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var person in e.NewFollowers)
                        {
                            newFollowers.Add(person.FromUserName);
                        }
                    });
                    await HaveShownAllNewFollowers();
                }
            }
        }

        async Task HaveShownAllNewFollowers()
        {
            while (newFollowers.Count > 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var follower = newFollowers[0];
                    newFollowers.RemoveAt(0);

                    TXTTopLeftMessage.Text = $"{follower} has just followed!";

                    METopLeftNotificationSound.Position = TimeSpan.Zero;
                    storyboard.Begin();
                    METopLeftNotificationSound.Play();

                    TopLeftNotificationShowing.Start();
                });
                
                await Task.Delay(6000);
            }
        }
    }
}
