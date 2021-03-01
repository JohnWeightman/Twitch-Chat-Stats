using System;
using System.Collections.Generic;
using Debugger;

namespace TwitchDeathCount
{
    static class Master
    {
        static List<Tuple<string, int>> Variables = new List<Tuple<string, int>>();
        static List<string> Commands = new List<string>() { "******" };

        static void Main(string[] args)
        {
            ConWin.ConWinStart();
            Twitch.LaunchConnection();
            Console.ReadLine();
        }

        #region Manage Commands

        #region Mechanics

        public static string ProcessInput(string Msg, string User)
        {
            byte Command = IsMsgCommand(Msg);
            if (Command == 255)
                return "Invalid Command - " + Msg;
            switch (Command)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    AddOrRemoveToVariableCount(Msg, Command);
                    break;
                case 15:
                    Msg = NewVariable(Msg);
                    break;
                case 16:
                    Msg = DeleteVariable(Msg);
                    break;
                default:
                    Debug.Log("Master - ProcessInput() -> Invalid Command: " + Command + " (" + Msg + ")", 1);
                    break;
            }
            return Msg;
        }

        static byte IsMsgCommand(string Msg)
        {
            for (byte VarPos = 0; VarPos < Variables.Count; VarPos++)
                if (Msg == "!" + Variables[VarPos].Item1)
                    return VarPos;
            if (Msg.Contains("!newvar"))
                return 15;
            else if (Msg.Contains("!delvar"))
                return 16;
            byte Val = 16;
            foreach(string Command in Commands)
            {
                Val++;
                if (Msg == Command)               
                    return Val;
            }
            return 255;
        }

        #endregion

        static void AddOrRemoveToVariableCount(string Msg, byte VarPos)
        {
            if (Msg.Contains('-'))
                Variables[VarPos] = new Tuple<string, int>(Variables[VarPos].Item1, Variables[VarPos].Item2 - 1);
            else
                Variables[VarPos] = new Tuple<string, int>(Variables[VarPos].Item1, Variables[VarPos].Item2 + 1);
            ConWin.UpdateVarBoxes(VarPos + 1, Variables[VarPos].Item1 + ": " + Variables[VarPos].Item2);
        }

        #region New Variable

        static string NewVariable(string Msg)
        {
            if(Variables.Count == 15)
            {
                Twitch.WriteToChat("Engine: Variables list is full, delete a variable before adding another!");
                return Msg + " -> Engine: Variables list is full!";
            }
            Tuple<string, int> NewTuple = new Tuple<string, int>(Msg.Substring(8, Msg.Length - 8), 0);
            if (!CheckForExistingVariable(NewTuple.Item1))
                Variables.Add(NewTuple);
            else
            {
                Twitch.WriteToChat("Engine: That variable already exists!");
                return Msg + " -> Engine: That variable already exists!";
            }
            ConWin.UpdateVarBoxes(Variables.Count, NewTuple.Item1 + ": 0");
            return Msg;
        }

        static bool CheckForExistingVariable(string NewVar)
        {
            foreach (Tuple<string, int> Var in Variables)
                if (NewVar == Var.Item1)
                    return true;
            return false;
        }

        #endregion

        #region Delete Variable

        static string DeleteVariable(string Msg)
        {
            if(Variables.Count < 1)
            {
                Twitch.WriteToChat("Engine: Variables list is empty, make a new variable!");
                return Msg + " -> Engine: Variables list is empty!";
            }
            int VarPos = FindVariablePosition(Msg.Substring(8, Msg.Length - 8));
            if (VarPos != -1)
                Variables.RemoveAt(VarPos);
            else
            {
                Twitch.WriteToChat("Engine: That variable doesn't exist!");
                return Msg + " -> Engine: That variable doesn't exist!";
            }
            UpdateAllVarBoxes();
            //ConWin.UpdateVarBoxes(VarPos + 1, "Var " + (VarPos + 1));
            return Msg;
        }

        static int FindVariablePosition(string DelVar)
        {
            int VarPos = 0;
            foreach(Tuple<string, int> Var in Variables)
            {
                if (DelVar == Var.Item1)
                    return VarPos;
                VarPos++;
            }
            return -1;
        }

        #endregion

        #endregion
    
        static void UpdateAllVarBoxes()
        {
            for (int x = 0; x < Variables.Count; x++)
                ConWin.UpdateVarBoxes(x + 1, Variables[x].Item1 + ": " + Variables[x].Item2);
            for (int x = 15 - (15 - Variables.Count); x < 15; x++)
                ConWin.UpdateVarBoxes(x + 1, "Var " + (x + 1));
        }
    }
}
