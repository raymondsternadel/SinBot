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
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Streams;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using SinBot.Views;
using System.IO;
using SinBot.CustomControls;
using System.Timers;
using SinBot.Model;
using OpenTriviaSharp;
using OpenTriviaSharp.Models;

namespace SinBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TwitchClient client;
        private TwitchAPI api;
        private FollowerService followerService;

        private Timer sinbotMentioned;

        public TriviaQuestion[] CurrentQuestion;

        private string commandString;

        // usage: system-wide keyboard hook
        private KeyboardHook _hook;

        public MainWindow()
        {
            BotSetup();

            sinbotMentioned = new Timer(10000);
            sinbotMentioned.AutoReset = false;
            sinbotMentioned.Elapsed += SendARandomCommandTimer_Elapsed;

            SessionContext.TwitchClient = client;
            SessionContext.TwitchAPI = api;
            SessionContext.TwitchFollowerService = followerService;

            InitializeComponent();

            // install system-wide keyboard hook
            _hook = new KeyboardHook();
            _hook.KeyDown += new KeyboardHook.HookEventHandler(OnHookKeyDown);

            SessionContext.SendARandomCommandTimer.Elapsed += SendARandomCommandTimer_Elapsed;
        }

        public void BotSetup()
        {
            var credentials = new ConnectionCredentials(Credentials.Username, Credentials.AccessToken);
            var clientOptions = new ClientOptions()
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            var customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, Credentials.ChannelName);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;
            client.OnExistingUsersDetected += Client_OnExistingUsersDetected;

            client.Connect();

            var scopes = new List<TwitchLib.Api.Core.Enums.AuthScopes>();
            scopes.Add(TwitchLib.Api.Core.Enums.AuthScopes.Any);

            api = new TwitchAPI();

            api.Settings.ClientId = Credentials.ClientID;
            api.Settings.Scopes = scopes;
            api.Settings.Secret = Credentials.Secret;

            followerService = new FollowerService(api);
            followerService.SetChannelsByName(new List<string>() { Credentials.ChannelName });

            followerService.OnNewFollowersDetected += FollowerService_OnNewFollowersDetected;
            followerService.Start();
        }

        public void LiveSetup()
        {
            SessionContext.FullscreenMediaElement.Pause();
            SessionContext.FullscreenWindow.Visibility = Visibility.Hidden;

            FrameMain.Content = new LivePage();

            if (SessionContext.ChatVisible)
                SessionContext.ChatPanelWindow.Visibility = Visibility.Visible;
            else
                SessionContext.ChatPanelWindow.Visibility = Visibility.Hidden;

            if (SessionContext.WebcamVisible)
                SessionContext.WebcamWindow.Visibility = Visibility.Visible;
            else
                SessionContext.WebcamWindow.Visibility = Visibility.Hidden;

            SessionContext.StatsWindow.Visibility = Visibility.Visible;
        }

        public void BeRightBackSetup()
        {
            SessionContext.FullscreenWindow.Visibility = Visibility.Visible;
            SessionContext.FullscreenMediaElement.Source = new Uri("E:\\SinBot\\SinBot\\MediaFiles\\NotLive\\BeRightBack.mp4");
            SessionContext.WebcamWindow.Visibility = Visibility.Hidden;
            SessionContext.FullscreenMediaElement.Play();
        }

        public void StartingSoonSetup()
        {
            SessionContext.FullscreenWindow.Visibility = Visibility.Visible;
            SessionContext.FullscreenMediaElement.Source = new Uri("E:\\SinBot\\SinBot\\MediaFiles\\NotLive\\StartingSoon.mp4");
            SessionContext.WebcamWindow.Visibility = Visibility.Hidden;
            SessionContext.FullscreenMediaElement.Play();
        }

        public void EndingSoonSetup()
        {
            SessionContext.FullscreenWindow.Visibility = Visibility.Visible;
            SessionContext.FullscreenMediaElement.Source = new Uri("E:\\SinBot\\SinBot\\MediaFiles\\NotLive\\EndingSoon.mp4");
            SessionContext.WebcamWindow.Visibility = Visibility.Hidden;
            SessionContext.FullscreenMediaElement.Play();
        }

        public void PollSetup()
        {
            FrameMain.Content = new PollSetupPage();
        }

        public void TopRightVideoCommandReceived(string[] files)
        {
            if (files.Length != 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var rnd = ThreadSafeRandom.ThisThreadsRandom.Next(0, files.Length);

                    var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                    SessionContext.SmallVideoPanelWindow.Left = desktopWorkingArea.Right - SessionContext.SmallVideoPanelWindow.Width + 10;
                    SessionContext.SmallVideoPanelWindow.Top = desktopWorkingArea.Top - 10;

                    SessionContext.SmallVideoPanel.Source = new Uri(files[rnd]);
                });
            }
        }

        public void SoundCommandReceived(string[] files)
        {
            if (files.Length != 0)
            {
                var rnd = ThreadSafeRandom.ThisThreadsRandom.Next(0, files.Length);

                App.Current.Dispatcher.Invoke(() =>
                {
                    MESoundBoard.Source = new Uri(files[rnd]);
                });
            }
        }

        public void PlayTopRightVideo(string filePath)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                SessionContext.SmallVideoPanelWindow.Left = desktopWorkingArea.Right - SessionContext.SmallVideoPanelWindow.Width;
                SessionContext.SmallVideoPanelWindow.Top = desktopWorkingArea.Top - 10;

                SessionContext.SmallVideoPanel.Source = new Uri(filePath);
            });
        }

        #region Twitch Events

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            //if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
            //    client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            //else
            //    client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            //if (e.WhisperMessage.Username == "my_friend")
            //    client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var wasBannedWordFound = false;

            var bannedWords = await DatabaseService.GetAllBannedWords();

            // Check message for banned words
            foreach (var word in bannedWords)
            {
                if (e.ChatMessage.Message.Contains(word.Word, StringComparison.OrdinalIgnoreCase))
                {
                    wasBannedWordFound = true;

                    client.DeleteMessage(e.ChatMessage.Channel, e.ChatMessage);
                    client.SendMessage(e.ChatMessage.Channel, "Banned word detected. Message deleted.");
                    //client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(1), "Curse word detected. 1 minute timeout.");
                    break;
                }
            }

            // If no banned words were found then add message to the chat log
            if (!wasBannedWordFound)
            {
                var commands = await DatabaseService.GetAllCommands();

                // Check message for valid commands
                foreach (var command in commands)
                {
                    if (e.ChatMessage.Message.Contains(command.TriggerPhrase, StringComparison.OrdinalIgnoreCase))
                    {
                        await ValidCommandReceived(command);
                        break;
                    }
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    var chatMessage = new NewChatMessage();
                    chatMessage.txtChatMessage.Inlines.Add(new Run(e.ChatMessage.Username + ": ") { Foreground = Brushes.CornflowerBlue, FontSize = 20 });

                    var strings = e.ChatMessage.Message.Split(' ').ToList();

                    foreach (var myString in strings)
                    {
                        if (e.ChatMessage.EmoteSet.Emotes.Count == 0)
                        {
                            chatMessage.txtChatMessage.Inlines.Add(myString + " ");
                        }
                        else
                        {
                            foreach (var emote in e.ChatMessage.EmoteSet.Emotes)
                            {
                                if (myString != emote.Name)
                                {
                                    chatMessage.txtChatMessage.Inlines.Add(myString + " ");
                                    break;
                                }
                                else
                                {
                                    chatMessage.txtChatMessage.Inlines.Add(new Image() { Source = SessionContext.GetImageSourceFromUrl(emote.ImageUrl), Height = 28, Width = 28 });
                                    chatMessage.txtChatMessage.Inlines.Add(" ");
                                    break;
                                }
                            }
                        }
                    }

                    SessionContext.AddNewChatMessage(chatMessage);
                });

                if (e.ChatMessage.Message.Contains("sinbot", StringComparison.OrdinalIgnoreCase) ||
                    e.ChatMessage.Message.Contains("Sinbot", StringComparison.OrdinalIgnoreCase) ||
                    e.ChatMessage.Message.Contains("SinBot", StringComparison.OrdinalIgnoreCase))
                {
                    sinbotMentioned.Stop();
                    sinbotMentioned.Start();
                }
            }
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            try
            {
                client.SendMessage(e.Channel, "SinBot is now active.");
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");

        }

        private void Client_OnExistingUsersDetected(object sender, OnExistingUsersDetectedArgs e)
        {
            var count = e.Users.Count;
        }

        private void FollowerService_OnNewFollowersDetected(object sender, TwitchLib.Api.Services.Events.FollowerService.OnNewFollowersDetectedArgs e)
        {
            foreach (var follower in followerService.KnownFollowers)
            {
                var count = follower.Value.Count();

                if (count != e.NewFollowers.Count)
                {
                    // TODO Show new follower notification
                }
            }
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var chat = new ChatWindow();
            chat.Show();
            SessionContext.ChatPanelWindow.Visibility = Visibility.Hidden;

            var webcam = new WebcamWindow();
            webcam.Show();
            SessionContext.WebcamWindow.Visibility = Visibility.Hidden;

            var smallVideoPanel = new SmallVideoPanel();
            smallVideoPanel.Show();

            var fullscreenVideoPanel = new FullScreenVideoPanel();
            fullscreenVideoPanel.Show();

            SessionContext.FullscreenWindow.Visibility = Visibility.Hidden;

            var infoPopup = new InformationWindow();
            infoPopup.Show();

            var topLeftNotification = new TopLeftNotification();
            topLeftNotification.Show();

            var statsWindow = new StatsWindow();
            statsWindow.Show();
            statsWindow.Visibility = Visibility.Hidden;
        }

        private void RBLive_Click(object sender, RoutedEventArgs e)
        {
            LiveSetup();
        }

        private void RBBeRightBack_Click(object sender, RoutedEventArgs e)
        {
            BeRightBackSetup();
        }

        private void RBStartingSoon_Click(object sender, RoutedEventArgs e)
        {
            StartingSoonSetup();
        }

        private void RBEndingSoon_Click(object sender, RoutedEventArgs e)
        {
            EndingSoonSetup();
        }

        private void RBPoll_Click(object sender, RoutedEventArgs e)
        {
            PollSetup();
        }

        private void RBExit_Click(object sender, RoutedEventArgs e)
        {
            // TODO create are you sure window
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private async void SendARandomCommandTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var command = await DatabaseService.GetRandomCommand();

            client.SendMessage(Credentials.ChannelName, command.TriggerPhrase);
            await ValidCommandReceived(command);
        }

        // keyboard hook handler
        private async void OnHookKeyDown(object sender, HookEventArgs e)
        {
            commandString += e.Key.ToString();

            var commands = await DatabaseService.GetAllCommands();

            // Check message for valid commands
            foreach (var command in commands)
            {
                if (commandString.Contains(command.TriggerPhrase.Remove(0, 1), StringComparison.OrdinalIgnoreCase))
                {
                    commandString = string.Empty;
                    await ValidCommandReceived(command);
                    break;
                }
            }

            //var command = await DatabaseService.GetCommandByTriggerKey(e.Key.ToString());

            //if (command != null)
            //    await ValidCommandReceived(command);
        }

        private async Task ValidCommandReceived(Command command)
        {
            string[] files;

            if (command.MediaType == "video")
            {
                files = Directory.GetFiles(command.Path);
                TopRightVideoCommandReceived(files);
            }
            else if (command.MediaType == "audio")
            {
                files = Directory.GetFiles(command.Path);
                SoundCommandReceived(files);
            }
            else if (command.MediaType == "message")
            {
                var commandMessages = await DatabaseService.GetCommandMessagesByCommandID(command.ID);
                var rnd = ThreadSafeRandom.ThisThreadsRandom.Next(0, commandMessages.Count);

                client.SendMessage(Credentials.ChannelName, commandMessages[rnd].Message);
            }
            else if (command.MediaType == "other")
            {
                if (command.TriggerPhrase == "!commands")
                {
                    var commands = await DatabaseService.GetAllCommands();
                    var message = string.Empty;

                    foreach (var cmd in commands.OrderBy((x) => x.TriggerPhrase))
                    {
                        message += cmd.TriggerPhrase + " ";
                    }

                    client.SendMessage(Credentials.ChannelName, message);
                }
                else if (command.TriggerPhrase == "!trivia")
                {
                    await ShowTriviaQuestion();
                }
            }
        }

        private async Task ShowTriviaQuestion()
        {
            CurrentQuestion = await SessionContext.TriviaClient.GetQuestionAsync(amount: 1, type: TriviaType.Multiple, difficulty: Difficulty.Any, category: Category.Television);

            client.SendMessage(Credentials.ChannelName, CurrentQuestion[0].Question);

            var answers = new List<string>();

            answers.Add(CurrentQuestion[0].CorrectAnswer);
            answers.AddRange(CurrentQuestion[0].IncorrectAnswers);

            answers.Shuffle();

            var myString = "| ";

            foreach (var answer in answers)
            {
                myString += answer + " | ";
            }

            client.SendMessage(Credentials.ChannelName, myString);
        }
    }
}
