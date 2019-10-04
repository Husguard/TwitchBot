using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using System.Threading;
using System.Runtime.InteropServices;
using System.Timers;


namespace TwitchBot
{
    class Program
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            //        while (true)
            //        {
            //             ShowWindow(GetConsoleWindow(), 0); // Скрыть.
            //         }
            Console.ReadLine();
            
        }
        
    }

    class Bot
    {
        public TwitchClient client;
        public int cooldown = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        private static System.Timers.Timer aTimer;
        private int bet;
        private Boolean winstreak;

        public Bot()
        {
            bet = 250;
            aTimer = new System.Timers.Timer(123000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            ConnectionCredentials credentials = new ConnectionCredentials("Husguard_Bot", "bxbcy1u097o4iblyjsky5wskqyh2ki");
            
            client = new TwitchClient();
            client.Initialize(credentials, "hakumai");            

            client.OnConnected += OnConnected;
            client.OnJoinedChannel += OnJoinedChannel;
            client.OnMessageReceived += OnMessageReceived;
            client.OnWhisperReceived += OnWhisperReceived;
            client.OnNewSubscriber += OnNewSubscriber;
            
            client.Connect();

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            client.SendMessage("hakumai", "!coin " + bet.ToString());
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }
        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if(e.ChatMessage.Username == "coolshiba" && e.ChatMessage.Message.Contains("VoteNay"))
            {
                Console.WriteLine("LOSE IS {0}", bet);
                winstreak = false;
                bet = bet * 2;
            }
            if(e.ChatMessage.Username == "coolshiba" && e.ChatMessage.Message.Contains("VoteYea") && winstreak == false)
            {
                Console.WriteLine("WIN IS {0}", bet);
                winstreak = true;
                bet = bet / 2;
            }
            Console.WriteLine("Received message from {0}", e.ChatMessage.Username);
            // client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");
        }

        private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }
    }
}