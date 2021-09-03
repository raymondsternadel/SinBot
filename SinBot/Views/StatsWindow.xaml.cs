using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;

namespace SinBot.Views
{
    /// <summary>
    /// Interaction logic for StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        public StatsWindow()
        {
            InitializeComponent();

            SessionContext.TwitchClient.OnUserJoined += TwitchClient_OnUserJoined;
            SessionContext.TwitchClient.OnUserLeft += TwitchClient_OnUserLeft;
            SessionContext.TwitchFollowerService.OnServiceTick += TwitchFollowerService_OnServiceTick;
        }

        private void TwitchFollowerService_OnServiceTick(object sender, TwitchLib.Api.Services.Events.OnServiceTickArgs e)
        {
            if (SessionContext.TwitchFollowerService.KnownFollowers.Count > 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var currentCount = int.Parse(LBLFollowerCount.Content.ToString());

                    if (currentCount != SessionContext.TwitchFollowerService.KnownFollowers.First().Value.Count)
                        LBLFollowerCount.Content = SessionContext.TwitchFollowerService.KnownFollowers.First().Value.Count;
                });
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width - 20;
            this.Top = desktopWorkingArea.Bottom / 2 - 200;

            SessionContext.StatsWindow = this;
        }

        private async void TwitchClient_OnUserJoined(object sender, TwitchLib.Client.Events.OnUserJoinedArgs e)
        {
            var stream = await SessionContext.TwitchAPI.Helix.Streams.GetStreamsAsync(userIds: new List<string>() { Credentials.UserID });

            var viewerCount = stream.Streams.FirstOrDefault()?.ViewerCount;

            App.Current.Dispatcher.Invoke(() =>
            {
                if (viewerCount != null)
                    LBLViewerCount.Content = viewerCount;
            });
        }

        private async void TwitchClient_OnUserLeft(object sender, TwitchLib.Client.Events.OnUserLeftArgs e)
        {
            var stream = await SessionContext.TwitchAPI.Helix.Streams.GetStreamsAsync(userIds: new List<string>() { Credentials.UserID });

            var viewerCount = stream.Streams.FirstOrDefault()?.ViewerCount;

            App.Current.Dispatcher.Invoke(() =>
            {
                if (viewerCount != null)
                    LBLViewerCount.Content = viewerCount;
            });
        }

        #region Keeping Window On Top

        private void Window_Deactivated(object sender, EventArgs e)
        {
            App.Current.MainWindow.Topmost = true;

            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(App.Current.MainWindow).Handle;

            // Intentionally do not await the result
            App.Current.Dispatcher.BeginInvoke(new Action(async () => await RetrySetTopMost(hwnd)));
        }

        private const int RetrySetTopMostDelay = 200;
        private const int RetrySetTopMostMax = 20;

        // The code below will retry several times before giving up. This always worked with one retry in my tests.
        private static async Task RetrySetTopMost(IntPtr hwnd)
        {
            for (int i = 0; i < RetrySetTopMostMax; i++)
            {
                await Task.Delay(RetrySetTopMostDelay);
                int winStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

                if ((winStyle & WS_EX_TOPMOST) != 0)
                {
                    break;
                }

                App.Current.MainWindow.Topmost = false;
                App.Current.MainWindow.Topmost = true;
            }
        }

        internal const int GWL_EXSTYLE = -20;
        internal const int WS_EX_TOPMOST = 0x00000008;

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hwnd, int index);

        #endregion
    }
}
