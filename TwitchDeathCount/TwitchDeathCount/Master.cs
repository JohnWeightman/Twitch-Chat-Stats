using System;
using System.Collections.Generic;
using Debugger;

namespace TwitchDeathCount
{
    static class Master
    {
        static List<Tuple<string, int>> Variables;
        static List<string> Commands = new List<string>() { "!new" };

        static void Main(string[] args)
        {
            ConWin.ConWinStart();
            Twitch.LaunchConnection();
            Console.ReadLine();
        }

        #region Manage Variables

        public static void ProcessInput(string Msg, string User)
        {
            if (!IsMsgCommand(Msg))
            {
                Debug.Log("Master - ProcessInput() -> Invalid Command: " + Msg, 1);
                return;
            }
        }

        static bool IsMsgCommand(string Msg)
        {
            bool IsCommand = false;
            foreach(string Command in Commands)
                if(Msg == Command)
                {
                    IsCommand = true;
                    break;
                }
            return IsCommand;
        }

        #endregion
    }
}
