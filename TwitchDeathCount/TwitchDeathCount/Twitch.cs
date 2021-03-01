using System;
using System.Net.Sockets;
using System.IO;
using Debugger;

namespace TwitchDeathCount
{
    static class Twitch
    {
        public static string DisplayName;
        public static string UserName;
        public static string AuthKey;

        private static TcpClient TwitchClient;
        private static StreamReader SReader;
        private static StreamWriter SWriter;

        #region Connect to Twitch

        public static void LaunchConnection()
        {
            GetTwitchDetails();
            ConnectToTwitch();
            var timer = new System.Threading.Timer(e => ReadChat(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        static void GetTwitchDetails()
        {
            ConWin.UpdateTwitchLog("Twitch Client: Getting Details...");
            DisplayName = File.ReadAllText("Channel Details\\Display Name.txt").ToLower();
            UserName = File.ReadAllText("Channel Details\\User Name.txt").ToLower();
            AuthKey = File.ReadAllText("Channel Details\\Auth Key.txt").ToLower();
        }

        static void ConnectToTwitch()
        {
            ConWin.UpdateTwitchLog("Twitch Client: Connecting...");
            TwitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
            SReader = new StreamReader(TwitchClient.GetStream());
            SWriter = new StreamWriter(TwitchClient.GetStream());
            SWriter.WriteLine("PASS " + AuthKey);
            SWriter.WriteLine("NICK " + UserName);
            SWriter.WriteLine("USER " + UserName + " 8 * :" + UserName);
            SWriter.WriteLine("JOIN #" + DisplayName);
            SWriter.Flush();
            string Response = SReader.ReadLine();
            if (Response.Contains("Welcome, GLHF"))
            {
                ConWin.UpdateTwitchLog("Twitch Client: Connected");
            }
            else
            {
                ConWin.UpdateTwitchLog("Twitch Client: Failed to Connect");
                Debug.Log("Twitch - ConnectToTwitch() -> Failed to Connect", 3);
                return;
            }
            ConWin.UpdateTwitchLog(Response);
            string Response2 = SReader.ReadLine();
            ConWin.UpdateTwitchLog(Response2);
        }

        #endregion

        #region Write/Read Chat

        public static void WriteToChat(string Msg)
        {
            SWriter.WriteLine("PRIVMSG #" + DisplayName + " :" + Msg);
            SWriter.Flush();
        }

        static void ReadChat()
        {
            if (!TwitchClient.Connected)
            {
                ConnectToTwitch();
                return;
            }
            if(TwitchClient.Available > 0)
            {
                var Msg = SReader.ReadLine();
                if (Msg.Contains("PING"))
                {
                    SWriter.WriteLine("PONG :tmi.twitch.tv");
                }
                else if (Msg.Contains("PRIVMSG"))
                {
                    var splitPoint = Msg.IndexOf("!", 1);
                    var ChatName = Msg.Substring(0, splitPoint);
                    ChatName = ChatName.Substring(1);
                    splitPoint = Msg.IndexOf(":", 1);
                    Msg = Msg.Substring(splitPoint + 1);
                    if (Msg.Substring(0, 1) == "!")
                    {
                        Msg = Master.ProcessInput(Msg.ToLower(), ChatName);
                        Msg = ChatName + ": " + Msg;
                    }
                    else
                        Msg = "";
                    //if (Msg.Substring(0, 1) == "!" && ChatName == DisplayName.ToLower())
                    //    StreamerChatCommands(Msg);
                }
                if(Msg != "")
                    ConWin.UpdateTwitchLog(Msg);
            }
            //if (!Settings.GetPause())
            //    ChatWriterTimer -= 1;
            //if (ChatWriterTimer <= 0)
            //{
            //    WriteToChat("Type '!' and the number of the option you wish to vote for!");
            //    ChatWriterTimer = Settings.GetChatWriterTime();
            //}
        }

        #endregion

        #region Twitch Commands

        static void StreamerChatCommands(string Msg)
        {
            //if ((Msg == "!Start" || Msg == "!start" || Msg == "!START") && Settings.GetPause())
            //{
            //    Settings.SetPause(false);
            //    Debug.Environment("GAME RESUMED");
            //}
            //else if ((Msg == "!Stop" || Msg == "!stop" || Msg == "!STOP") && !Settings.GetPause())
            //{
            //    Settings.SetPause(true);
            //    Debug.Environment("GAME PAUSED");
            //}
        }

        #endregion

    }
}
