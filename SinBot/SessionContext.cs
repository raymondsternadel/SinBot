using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SinBot.Views;
using SinBot.Misc;
using System.Timers;
using SinBot.Model;
using TwitchLib.Client;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using System.Windows.Media.Imaging;
using OpenTriviaSharp;

namespace SinBot
{
    public static class SessionContext
    {
        #region Twitch Variables
        
        public static TwitchClient TwitchClient;
        public static TwitchAPI TwitchAPI;
        public static FollowerService TwitchFollowerService;
        
        #endregion

        #region Live Page Variables

        public static bool ChatVisible = true;
        public static bool WebcamVisible = true;
        public static Window WebcamWindow;
        public static StackPanel ChatPanel;
        public static Window ChatPanelWindow;
        public static MediaElement SmallVideoPanel;
        public static Window SmallVideoPanelWindow;
        public static List<Timer> ChatTimers;
        public static Timer InformationalPopupTimer;
        public static Window StatsWindow;

        #endregion

        #region Chat Timers

        public static Timer SendARandomCommandTimer;

        #endregion

        #region Not Live Variables

        public static Window FullscreenWindow;
        public static MediaElement FullscreenMediaElement;

        #endregion

        #region Poll Variables

        public static List<PollChoice> PollChoices;

        #endregion

        #region Trivia Variables

        public static OpenTriviaClient TriviaClient;

        #endregion

        static SessionContext()
        {
            ChatTimers = new List<Timer>();
            PollChoices = new List<PollChoice>();
            TriviaClient = new OpenTriviaClient();
            
            InformationalPopupTimer = new Timer(300000);
            InformationalPopupTimer.AutoReset = true;
            InformationalPopupTimer.Start();

            SendARandomCommandTimer = new Timer(550000);
            SendARandomCommandTimer.AutoReset = true;
            SendARandomCommandTimer.Start();
        }

        public static void AddNewChatMessage(NewChatMessage newChatMessage)
        {
            ChatPanel.Children.Add(newChatMessage);

            var timer = new Timer();

            timer.Interval = 120000;
            timer.AutoReset = false;
            timer.Elapsed += TimerElapsed;
            timer.Start();

            ChatTimers.Add(timer);
        }

        public static void TimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ChatPanel.Children.RemoveAt(0);
                ChatTimers.RemoveAt(0);
            });
        }

        public static BitmapImage GetImageSourceFromUrl(string URL)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(URL, UriKind.Absolute);
            bitmap.EndInit();

            return bitmap;
        }
    }
}
