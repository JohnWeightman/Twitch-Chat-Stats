﻿using System;
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
                case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10: case 11: case 12: case 13: case 14:
                    Msg = AddOrRemoveToVariableCount(Msg, Command);
                    break;
                case 15:
                    Msg = NewVariable(Msg);
                    break;
                case 16:
                    Msg = DeleteVariable(Msg);
                    break;
                case 17:
                    Msg = RenameVariable(Msg);
                    break;
                case 254:
                    return "Invalid Command - " + Msg;
                default:
                    Debug.Log("Master - ProcessInput() -> Invalid Command: " + Command + " (" + Msg + ")", 1);
                    break;
            }
            return Msg;
        }

        static byte IsMsgCommand(string Msg)
        {
            Msg = RemoveSymbols(Msg);
            for (byte VarPos = 0; VarPos < Variables.Count; VarPos++)
                if (Msg == "!" + Variables[VarPos].Item1)
                    return VarPos;
            if (Msg.IndexOf("\\", 1, 1) == 1)
                return GetPositionFromPositionCommand(Msg);
            else if (Msg.Contains("!newvar"))
                return 15;
            else if (Msg.Contains("!delvar"))
                return 16;
            else if (Msg.Contains("!rename"))
                return 17;
            byte Val = 17;
            foreach(string Command in Commands)
            {
                Val++;
                if (Msg == Command)               
                    return Val;
            }
            return 255;
        }

        static byte GetPositionFromPositionCommand(string Msg)
        {
            try
            {
                return Convert.ToByte(Convert.ToInt32(Msg.Substring(2, Msg.Length - 2)) - 1);
            }
            catch
            {
                return 254;
            }
        }

        static string RemoveSymbols(string Msg)
        {
            int Index = 0;
            if (Msg.Contains("+"))
            {
                Index = Msg.IndexOf("+", 0, Msg.Length);
                Msg = Msg.Substring(0, Index);
                Msg = Msg.Replace(" ", string.Empty);
            }
            else if (Msg.Contains("-"))
            {
                Index = Msg.IndexOf("-", 0, Msg.Length);
                Msg = Msg.Substring(0, Index);
                Msg = Msg.Replace(" ", string.Empty);
            }
            else if (Msg.Contains("="))
            {
                Index = Msg.IndexOf("=", 0, Msg.Length);
                Msg = Msg.Substring(0, Index);
                Msg = Msg.Replace(" ", string.Empty);
            }
            return Msg;
        }

        #endregion

        static string AddOrRemoveToVariableCount(string Msg, byte VarPos)
        {
            try
            {
                if (Msg.Contains("+"))
                {
                    int Index = Msg.IndexOf("+", 0, Msg.Length);
                    int Number = Convert.ToInt32(Msg.Substring(Index + 1, Msg.Length - (Index + 1)));
                    Variables[VarPos] = new Tuple<string, int>(Variables[VarPos].Item1, Variables[VarPos].Item2 + Number);
                }
                else if (Msg.Contains("-"))
                {
                    int Index = Msg.IndexOf("-", 0, Msg.Length);
                    int Number = Convert.ToInt32(Msg.Substring(Index + 1, Msg.Length - (Index + 1)));
                    Variables[VarPos] = new Tuple<string, int>(Variables[VarPos].Item1, Variables[VarPos].Item2 - Number);
                }
                else if (Msg.Contains("="))
                {
                    int Index = Msg.IndexOf("=", 0, Msg.Length);
                    int Number = Convert.ToInt32(Msg.Substring(Index + 1, Msg.Length - (Index + 1)));
                    Variables[VarPos] = new Tuple<string, int>(Variables[VarPos].Item1, Number);
                }
                else
                {
                    Variables[VarPos] = new Tuple<string, int>(Variables[VarPos].Item1, Variables[VarPos].Item2 + 1);
                }
                ConWin.UpdateVarBoxes(VarPos + 1, Variables[VarPos].Item1 + ": " + Variables[VarPos].Item2);
                IO.UpdateMainVariableFile(Variables);
            }
            catch
            {
                Debug.Log("Master - AddOrRemoveToVariableCount() -> Error Processing Command: " + VarPos + " (" + Msg + ")", 1);
                Twitch.WriteToChat("Engine: Variable Error!");
                return " -> Engine: Variable Error!";
            }
            return Msg;
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
            IO.UpdateMainVariableFile(Variables);
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

        #region Rename Variable

        static string RenameVariable(string Msg)
        {
            string Command = Msg.Substring(8, Msg.Length - 8);
            int Index = Command.IndexOf("-", 0, Command.Length);
            string CurrentName = Command.Substring(0, Command.Length - Index);
            string NewName = Command.Substring(Index + 1, Command.Length - (Index + 1));
            for(int x = 0; x < Variables.Count; x++)
                if(Variables[x].Item1 == CurrentName)
                {
                    Variables[x] = new Tuple<string, int>(NewName, Variables[x].Item2);
                    ConWin.UpdateVarBoxes(x + 1, Variables[x].Item1 + ": " + Variables[x].Item2);
                    return Msg;
                }
            Twitch.WriteToChat("Engine: The variable '" + CurrentName + "' Doesn't exist!");
            return " -> Engine: The variable '" + CurrentName + "' Doesn't exist!";
        }

        #endregion

        #endregion

        static void UpdateAllVarBoxes()
        {
            for (int x = 0; x < Variables.Count; x++)
                ConWin.UpdateVarBoxes(x + 1, Variables[x].Item1 + ": " + Variables[x].Item2);
            for (int x = 15 - (15 - Variables.Count); x < 15; x++)
                ConWin.UpdateVarBoxes(x + 1, "Var " + (x + 1));
            IO.UpdateMainVariableFile(Variables);
        }
    }
}
